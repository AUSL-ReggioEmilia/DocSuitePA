using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.Sign.Interfaces;
using VecompSoftware.Sign.Models;

namespace VecompSoftware.Sign.ArubaSignService.Factories
{
    public class ArubaSignFactory : ISignFactory
    {
        #region [ Fields ]
        private const int _retry_tentative = 10;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(1);
        private readonly Aruba.ArubaSignServiceClient _arubaSignServiceClient;
        private string CurrentSessionId = string.Empty;

        private readonly Action<string> _externalInfoLogger;
        private readonly Action<string> _externalErrorLogger;
        #endregion

        #region [ Constructor ]
        public ArubaSignFactory(Action<string> externalInfoLogger, Action<string> externalErrorLogger)
        {
            _externalInfoLogger = externalInfoLogger;
            _externalErrorLogger = externalErrorLogger;

            _arubaSignServiceClient = new Aruba.ArubaSignServiceClient();
        }
        #endregion

        #region [ Methods ]
        private byte[] RetryingPolicyAction(Func<byte[]> func, int step = 1)
        {
            _externalInfoLogger($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress...");
            if (step >= _retry_tentative)
            {
                _externalErrorLogger("VecompSoftware.Services.SignService.ArubaSignService.Factories.RetryingPolicyAction: retry policy expired maximum tentatives");
                throw new Exception("ArubaSignServiceClient retry policy expired maximum tentatives");
            }
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                _externalErrorLogger($"ArubaSignServiceClient error: {ex.Message}");
                _externalErrorLogger(ex.StackTrace);
                _externalInfoLogger($"SafeActionWithRetryPolicy : tentative {step}/{_retry_tentative} faild. Waiting {_threadWaiting} second before retrying action");
                Task.Delay(_threadWaiting).Wait();
                return RetryingPolicyAction(func, ++step);
            }
        }


        public byte[] SignDocument(ArubaSignModel signModel, byte[] document, string filename, bool requiredMark)
        {
            _externalInfoLogger($"Aruba Sign Document request with document {document.Length}");
            ArubaSignModel signerModel = signModel;
            Aruba.signRequestV2 requestV2 = null;

            requestV2 = new Aruba.signRequestV2
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
                tsa_identity = new Aruba.tsaAuth()
                {
                    user = signerModel.TSAUser,
                    password = signerModel.TSAPassword
                },
                requiredmark = requiredMark,//per la marca temporale
                stream = document,
                transport = Aruba.typeTransport.STREAM,
            };

            return RetryingPolicyAction(() =>
            {
                Aruba.signReturnV2 response = _arubaSignServiceClient.pkcs7signV2(requestV2, false, true);
                if (response.status == "OK")
                {
                    _externalInfoLogger($"Document has been successfully signed by ARUBA ARSS.");
                    return response.stream;
                }
                _externalInfoLogger($"ARUBA ARSS failed with status {response.status} > {response.description}");
                throw new Exception(response.description);
            });

        }

        public ICollection<FileModel> SignDocuments(ArubaSignModel signModel, IEnumerable<FileModel> documents, bool requiredMark)
        {
            ArubaSignModel signerModel = signModel;
           
            List<FileModel> results = new List<FileModel>();
            FileModel signedFile;
            foreach (FileModel file in documents)
            {
                signedFile = new FileModel
                {
                    Filename = $"{file.Filename}.p7m",
                    Document = SignDocument(signModel, file.Document, file.Filename, requiredMark)
                };
                results.Add(signedFile);
            }
            return results;
        }

      
    }

    #endregion
}
