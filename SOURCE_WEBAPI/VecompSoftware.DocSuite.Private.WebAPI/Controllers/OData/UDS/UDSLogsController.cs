using System;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.UDS
{
    public class UDSLogsController : BaseODataController<UDSLog, IUDSLogService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Contructor ]
        public UDSLogsController(IUDSLogService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetMyLogs(Guid idUDS, int skip, int top)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IQueryable<UDSLog> result = _unitOfWork.Repository<UDSLog>().GetMyLogs(idUDS, skip, top, Domain, Username);
                return Ok(result);
            }, _logger, LogCategories);
        }
        #endregion
    }
}