using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Templates;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Templates
{
    public class TemplateDocumentRepositoriesController : BaseODataController<TemplateDocumentRepository, ITemplateDocumentRepositoryService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public TemplateDocumentRepositoriesController(ITemplateDocumentRepositoryService service, IDataUnitOfWork unitOfWork, ILogger logger,
            ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetTags(ODataQueryOptions<TemplateDocumentRepository> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<string> results = _unitOfWork.Repository<TemplateDocumentRepository>().GetTags();
                return Ok(results);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
