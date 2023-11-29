using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public class UDSCollaborationService : BaseService<UDSCollaboration>, IUDSCollaborationService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICollaborationService _collaborationService;
        #endregion

        #region [ Constructor ]
        public UDSCollaborationService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validatorService,
            IUDSRuleset udsRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security, ICollaborationService collaborationService)
            : base(unitOfWork, logger, validatorService, udsRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _collaborationService = collaborationService;
        }
        #endregion

        #region [ Methods ]
        private void CreateCollaborationRelationLog(UDSCollaboration entity)
        {
            CollaborationLog collaborationLog = new CollaborationLog()
            {
                LogType = CollaborationLogType.MODIFICA_SEMPLICE,
                LogDescription = string.Format("Collaborazione collegata ad archivio {0} con ID {1}", entity.Repository.Name, entity.IdUDS),
                LogDate = DateTime.UtcNow,
                SystemComputer = Environment.MachineName,
                Entity = entity.Relation
            };

            _unitOfWork.Repository<CollaborationLog>().Insert(collaborationLog);
        }

        protected override UDSCollaboration BeforeCreate(UDSCollaboration entity)
        {
            if (entity.Repository != null)
            {
                entity.Repository = _unitOfWork.Repository<UDSRepository>().Find(entity.Repository.UniqueId);
            }

            if (entity.Relation != null)
            {
                Collaboration collaboration = _unitOfWork.Repository<Collaboration>().Find(entity.Relation.EntityId);
                collaboration.IdStatus = CollaborationStatusType.Registered;
                _unitOfWork.Repository<Collaboration>().Update(collaboration);
                entity.Relation = collaboration;
                CreateCollaborationRelationLog(entity);
            }

            return base.BeforeCreate(entity);
        }

        protected override UDSCollaboration BeforeUpdate(UDSCollaboration entity, UDSCollaboration entityTransformed)
        {

            if (entity.Repository != null)
            {
                entityTransformed.Repository = _unitOfWork.Repository<UDSRepository>().Find(entity.Repository.UniqueId);
            }

            if (entity.Relation != null)
            {
                Collaboration collaboration = _unitOfWork.Repository<Collaboration>().Find(entity.Relation.EntityId);
                collaboration.IdStatus = CollaborationStatusType.Registered;
                _unitOfWork.Repository<Collaboration>().Update(collaboration);
                entity.Relation = collaboration;
                CreateCollaborationRelationLog(entity);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
