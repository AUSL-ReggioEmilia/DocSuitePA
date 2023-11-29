using System;
using System.Linq;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Collaborations;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations
{
    public class CollaborationService : BaseService<Collaboration>, ICollaborationService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private const string CREATE_LOG_DESCRIPTION = "Inserimento della collaborazione da WebAPI";
        private const string UPDATE_LOG_DESCRIPTION = "Modifica della collaborazione da WebAPI";
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public CollaborationService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ICollaborationRuleset collaborationRuleset, IMapperUnitOfWork mapperUnitOfWork, IDecryptedParameterEnvService parameterEnvService, ISecurity security)
            : base(unitOfWork, logger, validationService, collaborationRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _parameterEnvService = parameterEnvService;
        }
        #endregion

        #region [ Methods ]
        public static CollaborationLog CreateLog(Collaboration collaboration, string logType, string logDescription, string registrationUser, int? idChain = null, short? incremental = null, int? collaborationIncremental = null)
        {
            CollaborationLog collaborationLog = new CollaborationLog()
            {
                LogType = logType,
                LogDescription = logDescription,
                LogDate = DateTime.Now,
                SystemComputer = Environment.MachineName,
                RegistrationUser = registrationUser,
                Program = "WA",
                IdChain = idChain,
                Incremental = incremental,
                CollaborationIncremental = collaborationIncremental,
                Entity = collaboration
            };

            return collaborationLog;
        }

        protected override Collaboration BeforeCreate(Collaboration entity)
        {
            Incremental lastIncremental = _unitOfWork.Repository<Incremental>().GetLastCollaborationIncremental().SingleOrDefault();
            if (lastIncremental != null)
            {
                lastIncremental.IncrementalValue = lastIncremental.IncrementalValue.HasValue ? lastIncremental.IncrementalValue.Value + 1 : 0;
                entity.EntityId = lastIncremental.IncrementalValue.Value;
                _unitOfWork.Repository<Incremental>().Update(lastIncremental);
            }

            short collaborationLocationId = _parameterEnvService.CollaborationLocationId;
            Location collaborationLocation = _unitOfWork.Repository<Location>().Find(collaborationLocationId);
            if (collaborationLocation != null)
            {
                entity.Location = _unitOfWork.Repository<Location>().Find(collaborationLocationId);
            }

            if (entity.WorkflowInstance != null)
            {
                entity.WorkflowInstance = _unitOfWork.Repository<WorkflowInstance>().Find(entity.WorkflowInstance.UniqueId);
            }

            if (entity.CollaborationSigns != null && entity.CollaborationSigns.Count > 0)
            {
                _unitOfWork.Repository<CollaborationSign>().InsertRange(entity.CollaborationSigns);
            }

            if (entity.CollaborationUsers != null && entity.CollaborationUsers.Count > 0)
            {
                foreach (CollaborationUser item in entity.CollaborationUsers)
                {
                    if (item.Role != null)
                    {
                        item.Role = _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId);
                    }
                }
                _unitOfWork.Repository<CollaborationUser>().InsertRange(entity.CollaborationUsers);
            }

            if (entity.CollaborationVersionings != null && entity.CollaborationVersionings.Count > 0)
            {
                _unitOfWork.Repository<CollaborationVersioning>().InsertRange(entity.CollaborationVersionings);
            }

            if (entity.CollaborationLogs != null && entity.CollaborationLogs.Count > 0)
            {
                _unitOfWork.Repository<CollaborationLog>().InsertRange(entity.CollaborationLogs);
            }

            //TODO: da spostare nel BaseLogService
            CollaborationLog collaborationLog = new CollaborationLog()
            {
                LogType = CollaborationLogType.MODIFICA_SEMPLICE,
                LogDescription = CREATE_LOG_DESCRIPTION,
                SystemComputer = Environment.MachineName,
                Entity = entity
            };

            _unitOfWork.Repository<CollaborationLog>().Insert(collaborationLog);
            return base.BeforeCreate(entity);
        }

        protected override bool ExecuteMappingBeforeUpdate()
        {
            return CurrentUpdateActionType != UpdateActionType.CollaborationManaged;
        }

        protected override Collaboration BeforeUpdate(Collaboration entity, Collaboration entityTransformed)
        {
            if (CurrentUpdateActionType == UpdateActionType.CollaborationManaged)
            {
                entityTransformed.Year = entity.Year;
                entityTransformed.Number = entity.Number;
                entityTransformed.IdStatus = CollaborationStatusType.Registered;
                entityTransformed.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().Find(entity.DocumentUnit.UniqueId);
            }

            //TODO: da spostare nel BaseLogService
            CollaborationLog collaborationLog = new CollaborationLog()
            {
                LogType = CollaborationLogType.MODIFICA_SEMPLICE,
                LogDescription = UPDATE_LOG_DESCRIPTION,
                SystemComputer = Environment.MachineName,
                Entity = entityTransformed
            };

            _unitOfWork.Repository<CollaborationLog>().Insert(collaborationLog);
            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override Collaboration BeforeDelete(Collaboration entity, Collaboration entityTransformed)
        {
            if (CurrentDeleteActionType == DeleteActionType.DeleteCollaboration)
            {
                if (entityTransformed.CollaborationVersionings != null && entityTransformed.CollaborationVersionings.Count > 0)
                {
                    _unitOfWork.Repository<CollaborationVersioning>().DeleteRange(entityTransformed.CollaborationVersionings.ToList());
                }
                if (entityTransformed.CollaborationUsers != null && entityTransformed.CollaborationUsers.Count > 0)
                {
                    _unitOfWork.Repository<CollaborationUser>().DeleteRange(entityTransformed.CollaborationUsers.ToList());
                }
                if (entityTransformed.CollaborationSigns != null && entityTransformed.CollaborationSigns.Count > 0)
                {
                    _unitOfWork.Repository<CollaborationSign>().DeleteRange(entityTransformed.CollaborationSigns.ToList());
                }
                if (entityTransformed.CollaborationAggregates != null && entityTransformed.CollaborationAggregates.Count > 0)
                {
                    _unitOfWork.Repository<CollaborationAggregate>().DeleteRange(entityTransformed.CollaborationAggregates.ToList());
                }
                if (entityTransformed.CollaborationLogs != null && entityTransformed.CollaborationLogs.Count > 0)
                {
                    _unitOfWork.Repository<CollaborationLog>().DeleteRange(entityTransformed.CollaborationLogs.ToList());
                }

                _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(
                    entityTransformed.UniqueId,
                    entityTransformed.EntityId,
                    TableLogEvent.DELETE,
                    $"Annullata collaborazione {entityTransformed.Subject} del {entityTransformed.RegistrationDate} ({entityTransformed.EntityId} - da {CurrentDomainUser.Account})",
                    typeof(Collaboration).Name,
                    CurrentDomainUser.Account)
                );
            }
            
            return base.BeforeDelete(entity, entityTransformed);
        }

        protected override IQueryFluent<Collaboration> SetEntityIncludeOnDelete(IQueryFluent<Collaboration> query)
        {
            if (CurrentDeleteActionType.HasValue && CurrentDeleteActionType == DeleteActionType.DeleteCollaboration)
            {
                query = query
                    .Include(f => f.CollaborationVersionings)
                    .Include(f => f.CollaborationUsers)
                    .Include(f => f.CollaborationSigns)
                    .Include(f => f.CollaborationAggregates)
                    .Include(f => f.CollaborationLogs);
            }
            return query;
        }
        #endregion

    }
}
