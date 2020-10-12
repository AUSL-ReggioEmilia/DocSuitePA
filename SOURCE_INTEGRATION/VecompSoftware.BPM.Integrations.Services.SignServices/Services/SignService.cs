using VecompSoftware.BPM.Integrations.Services.SignServices.ArubaService.Factories;
using VecompSoftware.BPM.Integrations.Services.SignServices.Enums;
using VecompSoftware.BPM.Integrations.Services.SignServices.Models;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Services.SignServices.Services
{
    public class SignService : ISignServiceClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public SignService(ILogger logger)
        {
            _logger = logger;
        }


        #endregion

        #region [ Methods ]
        public byte[] SignDocument(ISignerParameter signerParameter, byte[] document, ProviderType signature = ProviderType.ArubaSign)
        {
            ISignFactory signer = null;

            switch (signature)
            {
                case ProviderType.ArubaSign:
                    signer = new ArubaSignFactory(_logger);
                    break;
                default:
                    break;
            }
            return signer.SignDocument(signerParameter, document);
        }

        #endregion
    }
}
