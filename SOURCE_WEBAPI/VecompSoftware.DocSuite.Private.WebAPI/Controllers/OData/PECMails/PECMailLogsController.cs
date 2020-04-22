using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.PECMails;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.PECMails
{
    public class PECMailLogsController : BaseODataController<PECMailLog, IPECMailLogService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IRepositoryAsync<PECMailLog> _repository;
        private readonly IPECMailLogService _service;

        #endregion

        #region [ Constructor ]

        public PECMailLogsController(IPECMailLogService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _service = service;
            _logger = logger;
            _repository = unitOfWork.Repository<PECMailLog>();
        }

        #endregion
    }
}
