using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;

namespace VecompSoftware.DocSuiteWeb.Service.Entity
{
    [LogCategory(LogCategoryDefinition.SERVICEENTITY)]
    public abstract class BaseLogService<TEntity, TLogEntity, TType> : BaseService<TEntity>, ILogService<TEntity, TLogEntity, TType>
        where TEntity : DSWBaseLogEntity<TLogEntity, TType>, new()
        where TLogEntity : DSWBaseEntity, new()
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public BaseLogService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService, IValidatorRuleset validatorRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, validatorRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Constructor

        #region [ Methods ]

        protected override TEntity BeforeCreate(TEntity entity)
        {
            if (entity.Entity != null)
            {
                entity.Entity = _unitOfWork.Repository<TLogEntity>().Find(entity.Entity.UniqueId);
            }

            return base.BeforeCreate(entity);
        }



        #endregion
    }
}