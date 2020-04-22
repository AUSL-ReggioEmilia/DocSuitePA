using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Dossiers;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers
{
    public class DossierFolderService : BaseDossierService<DossierFolder>, IDossierFolderService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public DossierFolderService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDossierRuleset dossierRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, dossierRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override DossierFolder BeforeCreate(DossierFolder entity)
        {
            if (entity.Dossier != null)
            {
                entity.Dossier = _unitOfWork.Repository<Dossier>().GetWithProcesses(entity.Dossier.UniqueId).SingleOrDefault();
            }

            if (entity.Category != null)
            {
                entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            entity.Status = entity.Status == DossierFolderStatus.DoAction ? DossierFolderStatus.DoAction : DossierFolderStatus.InProgress;
            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
                entity.Status = DossierFolderStatus.Fascicle;
                CreateOrUpdateFolderHystoryLog(entity);
                entity.Name = string.Concat(entity.Fascicle.Title, "-", entity.Fascicle.FascicleObject);
                _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entity.Dossier, entity, DossierLogType.FolderModify,
                    string.Concat("Aggiornata la cartella '", entity.Name, "' con fascicolo associato '", entity.Fascicle.Title, "-", entity.Fascicle.FascicleObject, "'"), CurrentDomainUser.Account));
            }

            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entity.Dossier, entity, DossierLogType.FolderInsert,
                string.Concat("Creata cartella '", entity.Name, "' ", entity.Fascicle != null ?
                        string.Concat(" con fascicolo associato '", entity.Fascicle.Title, "-", entity.Fascicle.FascicleObject, "'") : string.Empty), CurrentDomainUser.Account));

            if (entity.DossierFolderRoles != null)
            {
                HashSet<DossierFolderRole> dossierFolderRoles = new HashSet<DossierFolderRole>();
                foreach (DossierFolderRole item in entity.DossierFolderRoles)
                {
                    dossierFolderRoles.Add(CreateDossierFolderRole(item, entity, _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId)));
                }
                entity.DossierFolderRoles = dossierFolderRoles.Where(f => f != null).ToList();
                _unitOfWork.Repository<DossierFolderRole>().InsertRange(entity.DossierFolderRoles);
            }

            if (entity.ParentInsertId.HasValue)
            {
                DossierFolder parentFolder = _unitOfWork.Repository<DossierFolder>().Find(entity.ParentInsertId);

                if (parentFolder != null && parentFolder.DossierFolderLevel > 1)
                {
                    parentFolder.Status = DossierFolderStatus.Folder;
                    _unitOfWork.Repository<DossierFolder>().Update(parentFolder);

                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entity.Dossier, parentFolder, DossierLogType.FolderModify,
                        string.Concat("Aggiornata la tipologia della cartella '", parentFolder.Name, "' da 'Cartella' a 'Cartella con sotto cartelle'"), CurrentDomainUser.Account));
                }
            }
            
            if (entity.Dossier != null)
            {
                foreach (Process item in entity.Dossier.Processes.Where(f=> f.ProcessType == ProcessType.Created))
                {
                    item.ProcessType = ProcessType.Defined;
                    _unitOfWork.Repository<Process>().Update(item);
                }
            }

            return base.BeforeCreate(entity);
        }

        private DossierFolderRole CreateDossierFolderRole(DossierFolderRole entity, DossierFolder dossierFolder, Role role)
        {
            DossierFolderRole dossierFolderRole = null;
            if (role != null)
            {
                dossierFolderRole = new DossierFolderRole
                {
                    Role = role,
                    DossierFolder = dossierFolder,
                    AuthorizationRoleType = AuthorizationRoleType.Accounted,
                    Status = DossierRoleStatus.Active,
                    IsMaster = true
                };

                _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(dossierFolder.Dossier, dossierFolder, DossierLogType.FolderAuthorize,
                    string.Concat("Autorizzata la cartella '", dossierFolder.Name, "' al settore '", role.Name, "' (", role.EntityShortId, ") per competenza (Accounted)"), CurrentDomainUser.Account));

                if (dossierFolder.Fascicle != null && !_unitOfWork.Repository<FascicleRole>().ExistFascicleRolesAccounted(dossierFolder.Fascicle.UniqueId, role.EntityShortId))
                {
                    CreateFascicleRole(dossierFolder, dossierFolderRole.AuthorizationRoleType, role);
                }

            }
            return dossierFolderRole;
        }

        protected override IQueryFluent<DossierFolder> SetEntityIncludeOnUpdate(IQueryFluent<DossierFolder> query)
        {
            query
                .Include(d => d.Fascicle)
                .Include(d => d.Dossier)
                .Include(d => d.DossierFolderRoles.Select(f => f.Role));
            return query;
        }

        private void CreateOrUpdateFolderHystoryLog(DossierFolder entityTransformed)
        {
            DossierLog logDossier = _unitOfWork.Repository<DossierLog>().GetFolderHystoryLog(entityTransformed.UniqueId).SingleOrDefault();
            DossierFolder toStore = new DossierFolder() { Name = entityTransformed.Name };

            string serializedEntity = JsonConvert.SerializeObject(toStore, DefaultJsonSerializer);
            if (logDossier == null)
            {
                _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed.Dossier, entityTransformed, DossierLogType.FolderHystory, serializedEntity, CurrentDomainUser.Account));
                return;
            }
            logDossier.LogDescription = serializedEntity;
            _unitOfWork.Repository<DossierLog>().Update(logDossier);
        }

        private void UpdateHashLog(DossierFolder entityTransformed)
        {
            ICollection<DossierLog> logsDossier = _unitOfWork.Repository<DossierLog>().GetLogsByHashValue(entityTransformed.Dossier.UniqueId.ToString()).ToList();
            ICollection<FascicleLog> logsFascicle = _unitOfWork.Repository<FascicleLog>().GetLogsByHashValue(entityTransformed.Dossier.UniqueId.ToString()).ToList();

            if (logsFascicle.Any())
            {
                foreach (FascicleLog logFascicle in logsFascicle)
                {
                    logFascicle.Hash = HashGenerator.GenerateHash(string.Concat(logFascicle.RegistrationUser, "|", logFascicle.LogType, "|", logFascicle.LogDescription, "|", logFascicle.UniqueId, "|", logFascicle.Entity.UniqueId, "|", logFascicle.RegistrationDate.ToString("yyyyMMddHHmmss")));
                    _unitOfWork.Repository<FascicleLog>().Update(logFascicle);
                }
            }

            if (logsDossier.Any())
            {
                foreach (DossierLog logDossier in logsDossier)
                {
                    logDossier.Hash = HashGenerator.GenerateHash(string.Concat(logDossier.RegistrationUser, "|", logDossier.LogType, "|", logDossier.LogDescription, "|", logDossier.UniqueId, "|", entityTransformed.Dossier.UniqueId, "|", logDossier.RegistrationDate.ToString("yyyyMMddHHmmss")));
                    _unitOfWork.Repository<DossierLog>().Update(logDossier);
                }
            }
        }

        private void DeleteFascicleRole(DossierFolderRole item, Guid? actualIdFascicle)
        {
            short roleId = item.Role.EntityShortId;
            string roleName = item.Role.Name;
            FascicleRole currentFascicleRole = null;
            if (actualIdFascicle.HasValue && (currentFascicleRole = _unitOfWork.Repository<FascicleRole>().GetFascicleRoleAccounted(actualIdFascicle.Value, roleId).SingleOrDefault()) != null)
            {
                _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(currentFascicleRole.Fascicle, FascicleLogType.Authorize,
                    string.Concat("Rimossa autorizzazione '", currentFascicleRole.AuthorizationRoleType.ToString(), "' al settore ", roleName, " (", roleId, ")"), CurrentDomainUser.Account));

                _unitOfWork.Repository<FascicleRole>().Delete(currentFascicleRole);

            }
        }

        private void CreateFascicleRole(DossierFolder dossierFolder, AuthorizationRoleType roleAuthorizationType, Role role)
        {
            FascicleRole fascicleRole = new FascicleRole()
            {
                Fascicle = dossierFolder.Fascicle,
                Role = role,
                AuthorizationRoleType = AuthorizationRoleType.Accounted
            };
            _unitOfWork.Repository<FascicleRole>().Insert(fascicleRole);

            _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(dossierFolder.Fascicle, FascicleLogType.Authorize,
                string.Concat("Aggiunta autorizzazione '", roleAuthorizationType.ToString(), "' al settore ", role.Name, " (", role.EntityShortId, ")"), CurrentDomainUser.Account));

            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(dossierFolder.Dossier, dossierFolder, DossierLogType.FolderAuthorize,
                string.Concat("Aggiunta autorizzazione '", roleAuthorizationType.ToString(), "' al settore '", role.Name,
                    "' (", role.EntityShortId, ") nel fascicolo '", dossierFolder.Fascicle.Title, "-", dossierFolder.Fascicle.FascicleObject, "'"), CurrentDomainUser.Account));
        }

        protected override DossierFolder BeforeUpdate(DossierFolder entity, DossierFolder entityTransformed)
        {
            bool hasFascicle = entityTransformed.Fascicle != null;
            entityTransformed.Status = DossierFolderStatus.InProgress;

            if (_unitOfWork.Repository<DossierFolder>().CountChildren(entity.UniqueId) > 0)
            {
                entityTransformed.Status = DossierFolderStatus.Folder;
            }

            Guid? actualIdFascicle = entityTransformed.Fascicle == null ? null : new Guid?(entityTransformed.Fascicle.UniqueId);
            entityTransformed.Fascicle = null;

            if (entity.Dossier != null)
            {
                entityTransformed.Dossier = _unitOfWork.Repository<Dossier>().Find(entity.Dossier.UniqueId);
            }

            if (entity.Category != null)
            {
                entityTransformed.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.Fascicle != null)
            {
                entityTransformed.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
                entityTransformed.Status = DossierFolderStatus.Fascicle;
                CreateOrUpdateFolderHystoryLog(entityTransformed);
                entityTransformed.Name = string.Concat(entityTransformed.Fascicle.Title, "-", entityTransformed.Fascicle.FascicleObject);
                _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed.Dossier, entityTransformed, DossierLogType.FolderModify,
                    string.Concat("Aggiornata la cartella '", entityTransformed.Name, "' con fascicolo associato '", entityTransformed.Fascicle.Title, "-", entityTransformed.Fascicle.FascicleObject, "'"), CurrentDomainUser.Account));
            }

            if (CurrentUpdateActionType.HasValue && CurrentUpdateActionType == Common.Infrastructures.UpdateActionType.DossierFolderAuthorizationsPropagation)
            {
                _unitOfWork.Repository<DossierFolder>().ExecuteProcedure(CommonDefinition.SQL_SP_DossierFolder_PropagateAtuthorizationToDescendants,
                    new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdParent, entity.UniqueId),
                    new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdDossier, entity.Dossier.UniqueId),
                    new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_AuthorizationType, AuthorizationRoleType.Accounted),
                    new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_AuthorizationTypeDescription, AuthorizationRoleType.Accounted.ToString()),
                    new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_RegistrationUser, CurrentDomainUser.Account),
                    new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_System, Environment.MachineName));

                UpdateHashLog(entityTransformed);
            }

            if (entityTransformed.Fascicle == null && hasFascicle)
            {

                foreach (DossierFolderRole item in entityTransformed.DossierFolderRoles.ToList())
                {
                    DeleteFascicleRole(item, actualIdFascicle);
                }

                DossierLog logDossier = _unitOfWork.Repository<DossierLog>().GetFolderHystoryLog(entityTransformed.UniqueId).SingleOrDefault();
                DossierFolder oldEntity = logDossier == null ? null : JsonConvert.DeserializeObject<DossierFolder>(logDossier.LogDescription, DefaultJsonSerializer);
                string previousFascileName = string.Copy(entityTransformed.Name);
                if (oldEntity != null)
                {
                    entityTransformed.Name = string.IsNullOrEmpty(oldEntity.Name) ? "Cartella vuota" : oldEntity.Name;
                }
                _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed.Dossier, entityTransformed, DossierLogType.FolderFascicleRemove,
                    string.Concat("Aggiornata la cartella '", entityTransformed.Name, "' con rimozione del fascicolo '", previousFascileName, "'"), CurrentDomainUser.Account));
            }

            if (entity.DossierFolderRoles != null)
            {
                foreach (DossierFolderRole item in entityTransformed.DossierFolderRoles.Where(f => !entity.DossierFolderRoles.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed.Dossier, entityTransformed, DossierLogType.FolderAuthorize,
                        string.Concat("Rimossa autorizzazione '", item.AuthorizationRoleType.ToString(), "' al settore '", item.Role.Name,
                            "' (", item.Role.EntityShortId, ") alla cartella '", entity.Name, "'"), CurrentDomainUser.Account));

                    DeleteFascicleRole(item, actualIdFascicle);
                    _unitOfWork.Repository<DossierFolderRole>().Delete(item);

                }
                DossierFolderRole newDossierFolderRole = null;
                foreach (DossierFolderRole item in entity.DossierFolderRoles.Where(f => !entityTransformed.DossierFolderRoles.Any(c => c.UniqueId == f.UniqueId)))
                {
                    newDossierFolderRole = CreateDossierFolderRole(item, entityTransformed, _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId));
                    if (newDossierFolderRole != null)
                    {
                        _unitOfWork.Repository<DossierFolderRole>().Insert(newDossierFolderRole);
                    }
                }
                Role role;
                foreach (DossierFolderRole item in entity.DossierFolderRoles.Where(f => entityTransformed.DossierFolderRoles.Any(c => c.UniqueId == f.UniqueId)))
                {
                    if (entityTransformed.Fascicle != null && !_unitOfWork.Repository<FascicleRole>().ExistFascicleRolesAccounted(entityTransformed.Fascicle.UniqueId, item.Role.EntityShortId))
                    {
                        role = _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId);
                        CreateFascicleRole(entityTransformed, AuthorizationRoleType.Accounted, role);
                    }
                }
            }

            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed.Dossier, entityTransformed, DossierLogType.FolderModify,
                string.Concat("Aggiornata la cartella '", entityTransformed.Name, "'"), CurrentDomainUser.Account));

            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override IQueryFluent<DossierFolder> SetEntityIncludeOnDelete(IQueryFluent<DossierFolder> query)
        {
            query.Include(d => d.Dossier)
                 .Include(d => d.DossierFolderRoles)
                 .Include(d => d.DossierLogs);
            return query;
        }

        protected override DossierFolder BeforeDelete(DossierFolder entity, DossierFolder entityTransformed)
        {
            if (entity.ParentInsertId.HasValue && entityTransformed.DossierFolderLevel > 2)
            {
                int countChildren = _unitOfWork.Repository<DossierFolder>().CountChildren(entity.ParentInsertId.Value);
                if (countChildren == 1)
                {
                    DossierFolder parentFolder = _unitOfWork.Repository<DossierFolder>().Find(entity.ParentInsertId);
                    parentFolder.Status = DossierFolderStatus.InProgress;
                    _unitOfWork.Repository<DossierFolder>().Update(parentFolder);

                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed.Dossier, parentFolder, DossierLogType.FolderModify,
                        string.Concat("Aggiornata la tipologia della cartella '", parentFolder.Name, "' da 'Cartella con sotto cartelle' a 'Cartella'"), CurrentDomainUser.Account));

                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed.Dossier, parentFolder, DossierLogType.FolderModify,
                        string.Concat("Aggiornata la cartella '", parentFolder.Name, "'"), CurrentDomainUser.Account));
                }
            }

            if (entityTransformed.DossierFolderRoles != null && entityTransformed.DossierFolderRoles.Any())
            {
                _unitOfWork.Repository<DossierFolderRole>().DeleteRange(entityTransformed.DossierFolderRoles.ToList());
            }

            if (entityTransformed.DossierLogs != null && entityTransformed.DossierLogs.Any())
            {
                foreach (DossierLog item in entityTransformed.DossierLogs)
                {
                    item.DossierFolder = null;
                }
            }

            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed.Dossier, null, DossierLogType.FolderDelete,
               string.Concat("Eliminata la cartella '", entityTransformed.Name, "'"), CurrentDomainUser.Account));

            return base.BeforeDelete(entity, entityTransformed);
        }
        #endregion
    }
}