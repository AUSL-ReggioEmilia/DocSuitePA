using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Service.Entity.Templates;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Templates
{
    public class TemplateReportController : BaseWebApiController<TemplateReport, ITemplateReportService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public TemplateReportController(ITemplateReportService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}