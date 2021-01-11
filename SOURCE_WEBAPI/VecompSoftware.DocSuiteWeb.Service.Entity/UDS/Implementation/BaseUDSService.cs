using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public abstract class BaseUDSService<TEntity> : BaseService<TEntity>
        where TEntity : DSWUDSBaseEntity, new()
    {

        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public BaseUDSService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IUDSRuleset ruleset, IMapperUnitOfWork mapper, ISecurity security)
            : base(unitOfWork, logger, validationService, ruleset, mapper, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]

        internal static UDSLog CreateUDSLog(Guid idUDS, UDSRepository repository, string logDescription, UDSLogType logType, string registrationUser, DateTimeOffset? logDate = null, string systemComputer = "")
        {
            UDSLog log = new UDSLog()
            {
                LogType = logType,
                LogDescription = logDescription,
                SystemComputer = string.IsNullOrEmpty(systemComputer) ? Environment.MachineName : systemComputer,
                Entity = repository,
                RegistrationDate = logDate.HasValue && logDate.Value != DateTimeOffset.MinValue ? logDate.Value : DateTimeOffset.UtcNow,
                RegistrationUser = registrationUser,
                IdUDS = idUDS,
                Environment = repository.DSWEnvironment,
            };
            log.Hash = HashGenerator.GenerateHash(string.Concat(log.RegistrationUser, "|", log.LogType, "|", log.LogDescription, "|", log.UniqueId, "|", idUDS, "|", log.RegistrationDate.ToString("yyyyMMddHHmmss")));
            return log;
        }

        protected virtual string GetLogMessage(TEntity entity)
        {
            return string.Empty;
        }

        protected override TEntity BeforeCreate(TEntity entity)
        {
            string message = GetLogMessage(entity);
            if (!string.IsNullOrEmpty(message))
            {
                _unitOfWork.Repository<UDSLog>().Insert(CreateUDSLog(entity.IdUDS, entity.Repository, $"Inserimento {message}", UDSLogType.AuthorizationInsert, CurrentDomainUser.Account));
            }

            return base.BeforeCreate(entity);
        }

        protected override TEntity BeforeDelete(TEntity entity, TEntity entityTransformed)
        {
            string message = GetLogMessage(entity);
            if (!string.IsNullOrEmpty(message))
            {
                _unitOfWork.Repository<UDSLog>().Insert(CreateUDSLog(entity.IdUDS, entity.Repository, $"Rimozione {message}", UDSLogType.AuthorizationDelete, CurrentDomainUser.Account));
            }

            return base.BeforeDelete(entity, entityTransformed);
        }

        #endregion
    }
}
