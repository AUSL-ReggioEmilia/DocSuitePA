using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.CustomModules
{
    [AllowAnonymous]
    public class AUSLRE_CommittenteDocumentsController : ApiController
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly Guid _instanceId;
        private readonly IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(AUSLRE_BandiDiGaraArchivesController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public AUSLRE_CommittenteDocumentsController(IDataUnitOfWork unitOfWork, ILogger logger, IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService)
            : base()
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _instanceId = Guid.NewGuid();
            _documentService = documentService;
        }
        #endregion

        #region [ Methods ]
        [HttpGet]
        public async Task<HttpResponseMessage> GetDocument(Guid guid)
        {
            return await CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                KeyValuePair<string, byte[]> documentDetails = await GetStreamOfDocument(guid);
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(documentDetails.Value)
                };

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = documentDetails.Key
                };

                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return result;
            }, _logger, _logCategories);
        }

        public async Task<KeyValuePair<string, byte[]>> GetStreamOfDocument(Guid uniqueId)
        {
            return await CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                ModelDocument.Document documentModel = await _documentService.GetDocumentAsync(uniqueId);
                bool result = _unitOfWork.Repository<UDSFieldList>().ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Get_UDS_T_Committente_HasDocument,
                new QueryParameter(CommonDefinition.SQL_Param_UDS_IdArchiveChain, documentModel.IdChain));
                if (!result)
                {
                    throw new UnauthorizedAccessException();
                }

                byte[] document = await _documentService.GetDocumentContentAsync(documentModel.IdDocument);
                return new KeyValuePair<string, byte[]>(documentModel.Name, document);
            }, _logger, _logCategories);
        }
        #endregion
    }
}