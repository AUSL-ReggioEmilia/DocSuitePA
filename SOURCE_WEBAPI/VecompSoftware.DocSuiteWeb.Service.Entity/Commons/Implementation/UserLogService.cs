using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
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
        private readonly string _passwordEncryptionKey;
        #endregion

        #region [ Constructor ]

        public UserLogService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IUserLogRuleset containerRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security, IEncryptionKey encryptionKey)
            : base(unitOfWork, logger, validationService, containerRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _security = security;
            _passwordEncryptionKey = encryptionKey.Value;
        }

        #endregion

        #region [ Methods ]
        protected override UserLog BeforeCreate(UserLog entity)
        {

            if (!string.IsNullOrEmpty(entity.UserProfile))
            {
                entity.UserProfile = EncryptionHelper.EncryptString(entity.UserProfile, _passwordEncryptionKey);
            }
            return base.BeforeCreate(entity);
        }
        protected override UserLog BeforeUpdate(UserLog entity, UserLog entityTransformed)
        {
            if (!string.IsNullOrEmpty(entity.UserProfile))
            {
                entityTransformed.UserProfile = EncryptionHelper.EncryptString(entity.UserProfile, _passwordEncryptionKey);
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
