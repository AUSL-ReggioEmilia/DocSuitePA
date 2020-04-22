using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public class UDSLogService : BaseService<UDSLog>, IUDSLogService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public UDSLogService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationervice,
            IUDSRuleset ruleset, IMapperUnitOfWork mapper, ISecurity security)
            : base(unitOfWork, logger, validationervice, ruleset, mapper, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override UDSLog BeforeCreate(UDSLog entity)
        {
            if (entity.Entity != null)
            {
                entity.Entity = _unitOfWork.Repository<UDSRepository>().Find(entity.Entity.UniqueId);
            }
            entity = BaseUDSService<UDSRole>.CreateUDSLog(entity.IdUDS, entity.Entity, entity.LogDescription, entity.LogType,
                string.IsNullOrEmpty(entity.RegistrationUser) ? CurrentDomainUser.Account : entity.RegistrationUser,
                logDate: entity.RegistrationDate, systemComputer: entity.SystemComputer);

            return base.BeforeCreate(entity);
        }

        protected override UDSLog BeforeUpdate(UDSLog entity, UDSLog entityTransformed)
        {
            throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
        }
        #endregion
    }
}
