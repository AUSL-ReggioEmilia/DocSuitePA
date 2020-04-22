using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Service.Entity.OCharts;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.OCharts
{
    public class OChartController : BaseWebApiController<OChart, IOChartService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public OChartController(IOChartService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}