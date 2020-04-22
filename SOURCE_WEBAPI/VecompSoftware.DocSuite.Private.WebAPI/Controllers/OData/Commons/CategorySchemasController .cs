using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Commons
{
    public class CategorySchemasController : BaseODataController<CategorySchema, ICategorySchemaService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]

        public CategorySchemasController(ICategorySchemaService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion
    }
}
