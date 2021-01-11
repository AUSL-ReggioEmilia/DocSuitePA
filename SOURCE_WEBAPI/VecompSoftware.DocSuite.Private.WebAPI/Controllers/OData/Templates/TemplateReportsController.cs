using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Templates;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Templates
{
    public class TemplateReportsController : BaseODataController<TemplateReport, ITemplateReportService>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public TemplateReportsController(ITemplateReportService service, IDataUnitOfWork unitOfWork, ILogger logger,
            ISecurity security)
            : base(service, unitOfWork, logger, security)
        {

        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}