using System;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Finder.PosteOnLineRequests;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.PosteWeb;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.PosteWeb
{
    public class POLRequestsController : BaseODataController<PosteOnLineRequest, IPOLRequestService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public POLRequestsController(IPOLRequestService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security) : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        public IHttpActionResult GetPOLRequest(Guid uniqueId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<PosteOnLineRequest> models = _unitOfWork.Repository<PosteOnLineRequest>().GetByUniqueId(uniqueId, optimization: true);

                return Ok(models);

            }, _logger, LogCategories);
        }

        public IHttpActionResult GetPOLRequestsByDocumentUnitId(Guid documentUnitId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<PosteOnLineRequest> models = _unitOfWork.Repository<PosteOnLineRequest>().GetByDocumentUnitId(documentUnitId, optimization: true);

                return Ok(models);

            }, _logger, LogCategories);
        }

        #endregion
    }
}