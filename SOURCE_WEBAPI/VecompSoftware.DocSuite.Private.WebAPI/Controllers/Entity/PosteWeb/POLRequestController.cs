using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Service.Entity.PosteWeb;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.PosteWeb
{
    [LogCategory(LogCategoryDefinition.WEBAPIENTITY)]
    public class POLRequestController : BaseWebApiController<PosteOnLineRequest, IPOLRequestService>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]

        protected POLRequestController(IPOLRequestService service, IDataUnitOfWork unitOfWork, ILogger logger) 
            : base(service, unitOfWork, logger)
        {
        }

        #endregion

        #region [ Methods ]
        #endregion
    }
}