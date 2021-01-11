using System;
using System.Collections.Generic;
using VecompSoftware.Sign.ArubaSignService;
using VecompSoftware.Sign.ArubaSignService.Factories;
using VecompSoftware.Sign.Interfaces;
using VecompSoftware.Sign.Models;

namespace VecompSoftware.Sign.Services
{
    public class SignService : ISignService
    {
        #region [ Fields ]
        private readonly Action<string> _externalInfoLogger;
        private readonly Action<string> _externalErrorLogger;
        #endregion

        #region [ Constructor ]
        public SignService(Action<string> externalInfoLogger, Action<string> externalErrorLogger)
        {
            _externalInfoLogger = externalInfoLogger;
            _externalErrorLogger = externalErrorLogger;
        }
        #endregion

        #region [ Methods ]
        public byte[] SignDocument(ArubaSignModel signModel, byte[] document, string filename, bool requiredMark)
        {
            return InitializeFactory().SignDocument(signModel,  document, filename, requiredMark);
        }

        public ICollection<FileModel> SignDocuments(ArubaSignModel signModel, IEnumerable<FileModel> documents, bool requiredMark)
        {
            return InitializeFactory().SignDocuments(signModel, documents, requiredMark);
        }

        private ArubaSignFactory InitializeFactory()
        {
            ArubaSignFactory signer = new ArubaSignFactory(_externalInfoLogger, _externalErrorLogger);
            _externalInfoLogger($"Initialize ArubaSign");
            return signer;
        }
        #endregion
    }
}
