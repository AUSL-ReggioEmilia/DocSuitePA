using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class UserLogService : BaseService<UserLog>, IUserLogService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ISecurity _security;
        #endregion

        #region [ Constructor ]

        public UserLogService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IUserLogRuleset containerRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, containerRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _security = security;
        }

        #endregion

        #region [ Methods ]
        #endregion

    }
}
