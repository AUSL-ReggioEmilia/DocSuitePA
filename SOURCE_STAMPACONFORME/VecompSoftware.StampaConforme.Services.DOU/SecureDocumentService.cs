using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.StampaConforme.Interfaces.Common.Services;
using VecompSoftware.StampConforme.Models.Commons;
using VecompSoftware.StampConforme.Models.SecureDocument;

namespace VecompSoftware.StampaConforme.Services.DOU
{
    [LogCategory(LogCategoryName.SECUREDOCUMENT)]
    public class SecureDocumentService : ISecureDocumentService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly DOUService.DOU _client;
        private readonly DOUResponseHelper _responseHelper;
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
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(SecureDocumentService));
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
        public SecureDocumentService(ILogger logger, int idService, string certificateThumbprint)
        {
            _logger = logger;
            _idService = idService;
            _certificateThumbprint = certificateThumbprint;
            _client = new DOUService.DOU();
            _client.ClientCertificates.Add(Certificate);
            ServicePointManager.ServerCertificateValidationCallback = (object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true;
            _responseHelper = new DOUResponseHelper();
        }
        #endregion

        #region [ Methods ]
        private void ValidateResponse(DOUService.DOUResponse response)
        {
            _logger.WriteDebug(new LogMessage("ValidateResponse -> validation received response"), LogCategories);
            if (!_responseHelper.ValidateResponse(response))
            {                
                string errorMessage = _responseHelper.ReadStatusMessage(response);
                _logger.WriteWarning(new LogMessage(string.Concat("ValidateResponse -> received error from DOU WS with status: ", response.status, " and description: ", errorMessage)), LogCategories);
                throw new Exception(string.Format("E' avvenuto un errore durante la richiesta di securizzazione. Codice: {0} - Descrizione: {1}", response.status, errorMessage));
            }
            _logger.WriteInfo(new LogMessage("ValidateResponse -> response from DOU WS is valid"), LogCategories);
        }

        public SecureDocumentModel Create(SecureDocumentModel documentModel)
        {
            try
            {
                _logger.WriteDebug(new LogMessage(string.Concat("Create -> create secure document for document ", documentModel.DocumentName)), LogCategories);
                if (documentModel.DocumentContent == null)
                {
                    _logger.WriteWarning(new LogMessage("Create -> document not found"), LogCategories);
                    throw new ArgumentNullException(nameof(SecureDocumentModel.DocumentContent), "Nessun documento passato per la securizzazione");
                }

                DOUService.DOUResponse response = _client.create(documentModel.DocumentContent, _idService, null);
                ValidateResponse(response);
                _logger.WriteInfo(new LogMessage(string.Concat("Create -> document generated correctly with id ", response.idPdf)), LogCategories);
                return new SecureDocumentModel()
                {
                    IdDocument = response.idPdf,
                    DocumentContent = response.pdf
                };
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("Create -> Error on create secure document call."), ex, LogCategories);
                throw;
            }
        }

        public SecureDocumentModel Update(SecureDocumentModel documentModel)
        {
            try
            {
                _logger.WriteDebug(new LogMessage(string.Concat("Update -> update secure document for iddocument ", documentModel.IdDocument)), LogCategories);
                if (documentModel.DocumentContent == null)
                {
                    _logger.WriteWarning(new LogMessage("Update -> document not found"), LogCategories);
                    throw new ArgumentNullException(nameof(SecureDocumentModel.DocumentContent), "Nessun documento passato per la securizzazione");
                }

                if (string.IsNullOrEmpty(documentModel.IdDocument))
                {
                    _logger.WriteWarning(new LogMessage("Update -> idDocument is null"), LogCategories);
                    throw new ArgumentNullException(nameof(SecureDocumentModel.IdDocument), "Nessun iddocument passato per la securizzazione");
                }

                DOUService.DOUResponse response = _client.update(documentModel.DocumentContent, documentModel.IdDocument, _idService);
                ValidateResponse(response);
                _logger.WriteInfo(new LogMessage(string.Concat("Update -> document with id ", response.idPdf," updated correctly")), LogCategories);
                return new SecureDocumentModel()
                {
                    IdDocument = response.idPdf,
                    DocumentContent = response.pdf
                };
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("Update -> Error on update secure document call."), ex, LogCategories);
                throw;
            }
        }

        public void Delete(SecureDocumentModel documentModel)
        {
            try
            {
                if (string.IsNullOrEmpty(documentModel.IdDocument))
                {
                    _logger.WriteWarning(new LogMessage("Delete -> idDocument is null"), LogCategories);
                    throw new ArgumentNullException(nameof(SecureDocumentModel.IdDocument), "Nessun iddocument passato per la securizzazione");
                }
                DOUService.DOUResponse response = _client.delete(documentModel.IdDocument, _idService);
                ValidateResponse(response);
                _logger.WriteInfo(new LogMessage(string.Concat("Delete -> document with id ", documentModel.IdDocument," deleted correctly")), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("Delete -> Error on delete secure document call."), ex, LogCategories);
                throw;
            }
        }

        public void Upload(SecureDocumentModel documentModel)
        {
            try
            {
                if (documentModel.DocumentContent == null)
                {
                    _logger.WriteWarning(new LogMessage("Upload -> document not found"), LogCategories);
                    throw new ArgumentNullException(nameof(SecureDocumentModel.DocumentContent), "Nessun documento passato per la securizzazione");
                }

                if (string.IsNullOrEmpty(documentModel.IdDocument))
                {
                    _logger.WriteWarning(new LogMessage("Upload -> idDocument is null"), LogCategories);
                    throw new ArgumentNullException(nameof(SecureDocumentModel.IdDocument), "Nessun iddocument passato per la securizzazione");
                }
                DOUService.DOUResponse response = _client.upload(documentModel.DocumentContent, documentModel.IdDocument, _idService, Encoding.UTF8.GetBytes(DEFAULT_METADATA));
                ValidateResponse(response);
                _logger.WriteInfo(new LogMessage(string.Concat("Upload -> document with id ", documentModel.IdDocument, " uploaded correctly")), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("Upload -> Error on upload secure document call."), ex, LogCategories);
                throw;
            }
        }
        #endregion
    }
}
