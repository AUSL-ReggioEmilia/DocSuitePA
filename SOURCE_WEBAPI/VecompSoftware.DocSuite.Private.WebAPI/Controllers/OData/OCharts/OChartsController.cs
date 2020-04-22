using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.OCharts;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.OCharts
{
    public class OChartsController : BaseODataController<OChart, IOChartService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public OChartsController(IOChartService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
        }

        #endregion
    }
}
