using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using VecompSoftware.DocSuite.SPID.Common.Logging;
using VecompSoftware.DocSuite.SPID.Model.SAML;
using VecompSoftware.DocSuite.SPID.AuthEngine.Helpers;
using VecompSoftware.DocSuite.SPID.AuthEngine.Models;

namespace VecompSoftware.DocSuite.SPID.AuthEngine.Auth
{
    public class RequestOptionFactory
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly AuthConfiguration _spidConfiguration;
        private readonly IdpHelper _idpHelper;
        #endregion

        #region [ Constructor ]
        public RequestOptionFactory(ILoggerFactory logger, IOptions<AuthConfiguration> spidConfiguration, IdpHelper idpHelper)
        {
            _logger = logger.CreateLogger(LogCategories.AUTHENGINE);
            _spidConfiguration = spidConfiguration?.Value;
            _idpHelper = idpHelper;
        }
        #endregion

        #region [ Methods ]
        public SamlRequestOption GenerateLogoutRequestOption(string idp)
        {
            string idpUrl = _idpHelper.GetSingleLogoutUrl(idp);
            return GenerateRequestOption(idp, idpUrl);
        }

        public SamlRequestOption GenerateAuthRequestOption(string idp)
        {
            string idpUrl = _idpHelper.GetSingleSignOnUrl(idp);
            return GenerateRequestOption(idp, idpUrl);
        }

        private SamlRequestOption GenerateRequestOption(string idp, string destinationUrl)
        {
            X509Certificate2 certificate = GetCertificate();
            if(certificate == null)
            {
                _logger.LogWarning("Nessun certificato trovato per la configurazione passata");
                return null;
            }

            SamlRequestOption samlRequestOption = new SamlRequestOption()
            {
                SPIDLevel = (SamlAuthLevel)_spidConfiguration.IdpAuthLevel,
                SPDomain = _spidConfiguration.SPDomain,
                AssertionConsumerServiceIndex = (ushort)_spidConfiguration.AssertionConsumerServiceIndex,
                AttributeConsumingServiceIndex = (ushort?)_spidConfiguration.AttributeConsumingServiceIndex,
                Destination = destinationUrl,
                Certificate = certificate,
                IdpEntityId = _idpHelper.GetEntityId(idp)
            };

            return samlRequestOption;
        }

        private X509Certificate2 GetCertificate()
        {
            X509Certificate2 cert = null;
            if (_spidConfiguration.CertificateFromFile && File.Exists(_spidConfiguration.CertificatePath))
            {
                cert = new X509Certificate2(File.ReadAllBytes(_spidConfiguration.CertificatePath), _spidConfiguration.CertificatePassword, X509KeyStorageFlags.Exportable);
            }
            else
            {
                using (X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadOnly))
                {
                    string cleanedThumbPrint = Regex.Replace(_spidConfiguration.CertificateThumbprint, @"\s|\W", "").ToUpper();
                    X509Certificate2Collection x509Certificates = x509Store.Certificates.Find(X509FindType.FindByThumbprint, cleanedThumbPrint, false);
                    if (x509Certificates.Count > 0)
                    {
                        cert = x509Certificates[0];
                    };
                }
            }
            return cert;
        }
        #endregion
    }
}
