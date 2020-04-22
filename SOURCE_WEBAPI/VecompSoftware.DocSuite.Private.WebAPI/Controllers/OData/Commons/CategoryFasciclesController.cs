using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Commons
{
    public class CategoryFasciclesController : BaseODataController<CategoryFascicle, ICategoryFascicleService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public CategoryFasciclesController(ICategoryFascicleService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GeAvailablePeriodicCategoryFascicles(ODataQueryOptions<CategoryFascicle> options, short idCategory)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CategoryFascicle> results = _unitOfWork.Repository<CategoryFascicle>().GeAvailablePeriodicCategoryFascicles(idCategory);
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult IsProcedureSecretary(short idCategory)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<RoleUser>().IsProcedureSecretary(Username, Domain, idCategory);
                return Ok(result);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
