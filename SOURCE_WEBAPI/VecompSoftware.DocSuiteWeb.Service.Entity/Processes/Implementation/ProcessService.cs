using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Processes
{
    public class ProcessService : BaseService<Process>, IProcessService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IDossierService _dossierService;
        private readonly IParameterEnvService _parameterEnvService;

        #endregion

        #region [ Constructor ]

        public ProcessService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IProcessRuleset processRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security,
            IDossierService dossierService, IParameterEnvService parameterEnvService)
            : base(unitOfWork, logger, validationService, processRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _dossierService = dossierService;
            _parameterEnvService = parameterEnvService;
        }

        #endregion

        #region [ Methods ]

        protected override Process BeforeCreate(Process entity)
        {
            entity.StartDate = DateTimeOffset.UtcNow;
            entity.ProcessType = ProcessType.Created;
            if (entity.Category != null)
            {
                entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.Roles != null)
            {
                HashSet<Role> roles = new HashSet<Role>();
                Role role = null;
                foreach (Role item in entity.Roles)
                {
                    role = _unitOfWork.Repository<Role>().Find(item.EntityShortId);
                    if (role != null)
                    {
                        roles.Add(role);
                    }
                }
                entity.Roles = roles;
            }

            Dossier defaultDossier = new Dossier()
            {
                Subject = entity.Name,
                Note = entity.Note,
                StartDate = DateTimeOffset.UtcNow
            };

            defaultDossier.Container = _unitOfWork.Repository<Container>().Find(_parameterEnvService.ProcessContainerId);
            defaultDossier.DossierRoles.Add(new DossierRole()
            {
                AuthorizationRoleType = AuthorizationRoleType.Responsible,
                Role = _unitOfWork.Repository<Role>().Find(_parameterEnvService.ProcessRoleId)
            });
            defaultDossier = Task.Run<Dossier>(async () =>
               {
                   return await _dossierService.CreateAsync(defaultDossier);
               }).Result;
            
            entity.Dossier = defaultDossier;
            _logger.WriteDebug(new LogMessage($"Generated automatically new dossier {defaultDossier.Subject}({defaultDossier.UniqueId}) to process {entity.Name} ({entity.UniqueId})"), LogCategories);

            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, $"Inserita raccolta dei procedimenti {entity.Name}", typeof(Process).Name, CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<Process> SetEntityIncludeOnUpdate(IQueryFluent<Process> query)
        {
            query.Include(d => d.Roles);
            return query;
        }

        protected override Process BeforeUpdate(Process entity, Process entityTransformed)
        {
            if (entity.Category != null)
            {
                entityTransformed.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.Roles != null)
            {
                foreach (Role item in entityTransformed.Roles.Where(f => !entity.Roles.Any(c => c.EntityShortId == f.EntityShortId)).ToList())
                {
                    entityTransformed.Roles.Remove(item);
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimosso il settore '", item.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                }
                foreach (Role item in entity.Roles.Where(f => !entityTransformed.Roles.Any(c => c.EntityShortId == f.EntityShortId)))
                {
                    entityTransformed.Roles.Add(_unitOfWork.Repository<Role>().Find(item.EntityShortId));
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunto il settore '", item.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                }
            }

            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.UPDATE, $"Modificata raccolta dei procedimenti {entity.Name} con ID {entity.UniqueId}", typeof(Process).Name, CurrentDomainUser.Account));

            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override bool ExecuteDelete()
        {
            return false;
        }

        protected override Process BeforeDelete(Process entity, Process entityTransformed)
        {
            if (CurrentDeleteActionType != Common.Infrastructures.DeleteActionType.CancelProcess)
            {
                throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
            }
            entityTransformed.EndDate = DateTimeOffset.UtcNow;
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, $"Rimosso raccolta dei procedimenti {entity.Name}", typeof(Process).Name, CurrentDomainUser.Account));

            return base.BeforeDelete(entity, entityTransformed);
        }

        #endregion

    }
}
