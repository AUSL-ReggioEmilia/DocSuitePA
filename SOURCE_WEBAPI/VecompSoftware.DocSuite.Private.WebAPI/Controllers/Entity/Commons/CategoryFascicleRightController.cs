using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Commons
{
    public class CategoryFascicleRightController : BaseWebApiController<CategoryFascicleRight, ICategoryFascicleRightService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public CategoryFascicleRightController(ICategoryFascicleRightService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}