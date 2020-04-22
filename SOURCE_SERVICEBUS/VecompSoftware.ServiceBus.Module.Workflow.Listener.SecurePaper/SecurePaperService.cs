using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.ServiceBus.Module.Workflow.Listener.SecurePaper
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class SecurePaperService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly SPService.SP _client;
        private readonly int _idService;
        private readonly string _certificateThumbprint;
        private X509Certificate2 _certificate;
        private static ICollection<LogCategory> _logCategories;
        private const string DEFAULT_METADATA = "<root></root>";
        #endregion

        #region [ Properties ]
        public int CurrentService => _idService;

        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(SecurePaperService));
                }
                return _logCategories;
            }
        }

        public X509Certificate2 Certificate
        {
            get
            {
                if (_certificate == null)
                {
                    using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                    {
                        store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                        X509Certificate2Collection collection = store.Certificates;
                        _certificate = collection.Find(X509FindType.FindByThumbprint, _certificateThumbprint, false)[0];
                    }
                }
                return _certificate;
            }
        }
        #endregion

        #region [ Constructor ]
        public SecurePaperService(ILogger logger, string url, int idService, string certificateThumbprint)
        {
            _logger = logger;
            _idService = idService;
            _certificateThumbprint = certificateThumbprint;
            Properties.Settings.Default.SecurePaperServiceUrl = url;
            _client = new SPService.SP();
            _client.ClientCertificates.Add(Certificate);
            ServicePointManager.ServerCertificateValidationCallback = (object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true;
        }
        #endregion

        #region [ Methods ]
        private void ValidateResponse(SPService.SPServiceResponse response)
        {
            _logger.WriteDebug(new LogMessage("ValidateResponse -> validation received response"), LogCategories);
            if (response.reason != "OK")
            {
                string errorMessage = response.reason;
                _logger.WriteWarning(new LogMessage(string.Concat("ValidateResponse -> received error from SecurePaper WS with status: ", response.status, " and description: ", errorMessage)), LogCategories);
                throw new Exception(string.Format("E' avvenuto un errore durante la richiesta di apposizione del contrassegno a stampa. Codice: {0} - Descrizione: {1}", response.status, errorMessage));
            }
            _logger.WriteInfo(new LogMessage("ValidateResponse -> response from SP WS is valid"), LogCategories);
        }

        public byte[] Create(string documentName, byte[] documentContent, out string archiveKey)
        {
            try
            {
                _logger.WriteDebug(new LogMessage(string.Concat("Create -> create secure paper for document ", documentName)), LogCategories);
                if (documentContent == null)
                {
                    _logger.WriteWarning(new LogMessage("Create -> document not found"), LogCategories);
                    throw new ArgumentNullException(nameof(documentContent), "Nessun documento passato per la securizzazione");
                }

                byte[] xmlMetadata = Encoding.UTF8.GetBytes("<root></root>");
                SPService.SPServiceResponse response = _client.securizeBufferOnPdf(_idService.ToString(), documentContent, documentContent, xmlMetadata, null);
                ValidateResponse(response);
                archiveKey = response.archiveKey;
                _logger.WriteInfo(new LogMessage(string.Concat("Create -> document ", documentName, " secure correctly")), LogCategories);
                return response.securedDocument;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("Create -> Error on create secure paper call."), ex, LogCategories);
                throw;
            }
        }
        #endregion
    }
}
