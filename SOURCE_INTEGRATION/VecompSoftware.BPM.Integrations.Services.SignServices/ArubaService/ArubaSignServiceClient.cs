using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Services.SignServices.ArubaService.Models;
using VecompSoftware.BPM.Integrations.Services.SignServices.Enums;
using VecompSoftware.BPM.Integrations.Services.SignServices.Models;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Services.SignServices.ArubaService
{
    [LogCategory(LogCategoryDefinition.DOCUMENTCONTEX)]
    public class ArubaSignServiceClient : ISignServiceClient
    {
        #region [ Fields ]
        private const int _retry_tentative = 10;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(1);
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly Aruba.ArubaSignServiceClient _arubaSignServiceClient;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ArubaSignServiceClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public ArubaSignServiceClient(ILogger logger)
        {
            _logger = logger;
            _arubaSignServiceClient = new Aruba.ArubaSignServiceClient();
        }
        #endregion

        #region [ Methods ]
        private byte[] RetryingPolicyAction(Func<byte[]> func, int step = 1)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress..."), LogCategories);
            if (step >= _retry_tentative)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.BPM.Integrations.Services.SignServices.ArubaService.RetryingPolicyAction: retry policy expired maximum tentatives"), LogCategories);
                throw new Exception("ArubaSignServiceClient retry policy expired maximum tentatives");
            }
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"SafeActionWithRetryPolicy : tentative {step}/{_retry_tentative} faild. Waiting {_threadWaiting} second before retrying action"), ex, LogCategories);
                Task.Delay(_threadWaiting).Wait();
                return RetryingPolicyAction(func, ++step);
            }
        }

        public byte[] SignDocument(ISignerParameter signerParameter, byte[] document, ProviderType signature = ProviderType.ArubaSign)
        {
            _logger.WriteDebug(new LogMessage($"SignDocument request with document {document.Length}"), LogCategories);

            if (!(signerParameter is SignerModel))
            {
                throw new ArgumentException($"SignDocument accecpt only {typeof(SignerModel)} argumemnt");
            }
            SignerModel signerModel = signerParameter as SignerModel;
            Aruba.signRequestV2 requestV2 = new Aruba.signRequestV2
            {
                certID = signerModel.CertificateId,
                identity = new Aruba.auth()
                {
                    delegated_domain = signerModel.DelegatedDomain,
                    delegated_password = signerModel.DelegatedPassword,
                    delegated_user = signerModel.DelegatedUser,
                    otpPwd = signerModel.OTPPassword,
                    typeOtpAuth = signerModel.OTPAuthType,
                    user = signerModel.User,
                },
                requiredmark = false,//per la marca temporale
                stream = document,
                transport = Aruba.typeTransport.STREAM,
            };

            return RetryingPolicyAction(() =>
            {
                Aruba.signReturnV2 response = _arubaSignServiceClient.pkcs7signV2(requestV2, false, true);
                if (response.status == "OK")
                {
                    _logger.WriteDebug(new LogMessage($"Document has been successfully signed by ARUBA ARSS."), LogCategories);
                    return response.stream;
                }
                _logger.WriteError(new LogMessage($"ARUBA ARSS failed with status {response.status} > {response.description}"), LogCategories);
                throw new Exception(response.description);
            });
        }
        #endregion        
    }
}
