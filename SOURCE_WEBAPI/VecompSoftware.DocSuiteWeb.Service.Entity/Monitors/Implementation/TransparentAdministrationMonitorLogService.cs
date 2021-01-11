using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Monitors;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Monitors
{
    public class TransparentAdministrationMonitorLogService : BaseService<TransparentAdministrationMonitorLog>, ITransparentAdministrationMonitorLogService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public TransparentAdministrationMonitorLogService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validatorService,
            ITransparentAdministrationMonitorLogRuleset transparentAdministrationMonitorLogRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validatorService, transparentAdministrationMonitorLogRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override TransparentAdministrationMonitorLog BeforeCreate(TransparentAdministrationMonitorLog entity)
        {
            if (entity.DocumentUnit != null)
            {
                entity.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().Find(entity.DocumentUnit.UniqueId);
            }

            if (entity.Role != null)
            {
                entity.Role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);
            }

            return base.BeforeCreate(entity);
        }

        protected override TransparentAdministrationMonitorLog BeforeUpdate(TransparentAdministrationMonitorLog entity, TransparentAdministrationMonitorLog entityTransformed)
        {

            if (entity.DocumentUnit != null)
            {
                entityTransformed.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().Find(entity.DocumentUnit.UniqueId);
            }

            if (entity.Role != null)
            {
                entityTransformed.Role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
