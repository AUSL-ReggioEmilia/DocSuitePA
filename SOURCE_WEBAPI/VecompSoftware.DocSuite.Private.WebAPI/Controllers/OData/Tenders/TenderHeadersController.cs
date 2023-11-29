using Microsoft.AspNet.OData;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tenders;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Tenders
{
    [EnableQuery]
    public class TenderHeadersController : BaseODataController<TenderHeader, ITenderHeaderService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public TenderHeadersController(ILogger logger, IDataUnitOfWork unitOfWork, ISecurity security, ITenderHeaderService service)
            :base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
