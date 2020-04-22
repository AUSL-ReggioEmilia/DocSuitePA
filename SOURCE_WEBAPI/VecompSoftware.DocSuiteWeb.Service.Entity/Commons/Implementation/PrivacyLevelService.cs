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
    public class PrivacyLevelService : BaseService<PrivacyLevel>, IPrivacyLevelService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public PrivacyLevelService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IPrivacyLevelRuleset privacyLevelRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, privacyLevelRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        protected override PrivacyLevel BeforeCreate(PrivacyLevel entity)
        {
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.PRIVACY, string.Format("Inserimento livello di privacy con livello {0}, attivo {1}", entity.Level, entity.IsActive), typeof(PrivacyLevel).Name, CurrentDomainUser.Account));
            return base.BeforeCreate(entity);
        }

        protected override PrivacyLevel BeforeUpdate(PrivacyLevel entity, PrivacyLevel entityTransformed)
        {
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.PRIVACY, string.Format("Modifica livello di privacy con livello {0}, attivo {1}", entityTransformed.Level, entityTransformed.IsActive), typeof(PrivacyLevel).Name, CurrentDomainUser.Account));
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion

    }
}