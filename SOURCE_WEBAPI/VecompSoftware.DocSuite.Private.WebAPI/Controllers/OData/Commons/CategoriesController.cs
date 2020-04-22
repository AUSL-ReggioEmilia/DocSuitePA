using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Commons
{
    public class CategoriesController : BaseODataController<Category, ICategoryService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        #endregion

        #region [ Constructor ]

        public CategoriesController(ICategoryService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security, IMapperUnitOfWork mapperUnitOfWork)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult FindCategory(ODataQueryOptions<Category> options, short idCategory, FascicleType? fascicleType)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CategoryFullTableValuedModel> categories = _unitOfWork.Repository<Category>().FindByIdCategory(Username, Domain, idCategory, fascicleType);
                ICollection<CategoryModel> results = _mapperUnitOfWork.Repository<IDomainMapper<CategoryFullTableValuedModel, CategoryModel>>().MapCollection(categories);
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult FindCategories(ODataQueryOptions<Category> options, [FromODataUri]CategoryFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<CategoryFullTableValuedModel> categories = _unitOfWork.Repository<Category>().FindCategories(Username, Domain, finder.Name, (FascicleType?)finder.FascicleType, finder.HasFascicleInsertRights,
                    finder.Manager, finder.Secretary, finder.IdRole, finder.LoadRoot, finder.ParentId, finder.ParentAllDescendants, finder.FullCode, finder.IdContainer, finder.FascicleFilterEnabled);
                ICollection<CategoryModel> results = _mapperUnitOfWork.Repository<IDomainMapper<CategoryFullTableValuedModel, CategoryModel>>().MapCollection(categories);
                return Ok(results);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
