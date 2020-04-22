using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Fascicles;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles
{
    public class FascicleService : BaseService<Fascicle>, IFascicleService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private bool _needdelete = true;
        #endregion

        #region [ Constructor ]

        public FascicleService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IFascicleRuleset fascicleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, fascicleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        public static FascicleLog CreateLog(Fascicle fascicle, FascicleLogType logType, string logDescription, string registrationUser)
        {
            FascicleLog fascicleLog = new FascicleLog()
            {
                LogType = logType,
                LogDescription = logDescription,
                SystemComputer = Environment.MachineName,
                RegistrationDate = DateTimeOffset.UtcNow,
                RegistrationUser = registrationUser,
                Entity = fascicle
            };
            fascicleLog.Hash = HashGenerator.GenerateHash(string.Concat(fascicleLog.RegistrationUser, "|", fascicleLog.LogType, "|", fascicleLog.LogDescription, "|", fascicleLog.UniqueId, "|", fascicle.UniqueId, "|", fascicleLog.RegistrationDate.ToString("yyyyMMddHHmmss")));
            return fascicleLog;
        }

        protected override Fascicle BeforeCreate(Fascicle entity)
        {
            if (entity.Contacts != null)
            {
                //entity.Contacts = _unitOfWork.Repository<Contact>().Queryable().Where(f => entity.Contacts.Any(x => x.EntityId == f.EntityId)).ToList();
                HashSet<Contact> contacts = new HashSet<Contact>();
                Contact contact = null;
                foreach (Contact item in entity.Contacts)
                {
                    contact = _unitOfWork.Repository<Contact>().Find(item.EntityId);
                    if (contact != null)
                    {
                        contacts.Add(contact);
                    }
                }
                entity.Contacts = contacts;
            }

            if (entity.FascicleDocuments != null && entity.FascicleDocuments.Count > 0)
            {
                _unitOfWork.Repository<FascicleDocument>().InsertRange(entity.FascicleDocuments);
            }

            if (entity.Category != null)
            {
                entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.MetadataRepository != null)
            {
                entity.MetadataRepository = _unitOfWork.Repository<MetadataRepository>().Find(entity.MetadataRepository.UniqueId);
            }

            if (entity.Container != null)
            {
                entity.Container = _unitOfWork.Repository<Container>().Find(entity.Container.EntityShortId);
            }

            entity.Number = -1;

            DateTimeOffset date = entity.StartDate;
            entity.StartDate = DateTimeOffset.UtcNow;

            if (CurrentInsertActionType.HasValue && CurrentInsertActionType.Value == InsertActionType.InsertPeriodicFascicle)
            {
                CategoryFascicle relatedCategoryFascicle = _unitOfWork.Repository<CategoryFascicle>().GetByEnvironment(entity.Category.EntityShortId, entity.DSWEnvironment.Value).FirstOrDefault();
                FasciclePeriod period = _unitOfWork.Repository<FasciclePeriod>().Find(relatedCategoryFascicle.FasciclePeriod.UniqueId);
                entity.StartDate = PeriodHelper.GetCurrentPeriodStart(period.PeriodName, date);
                if (entity.FascicleObject.Contains(string.Concat(" - ", period.PeriodName)))
                {
                    entity.FascicleObject = entity.FascicleObject.Replace(entity.FascicleObject.Split('-').LastOrDefault(), "");
                }
                entity.FascicleObject = string.Concat(entity.FascicleObject, " - ", period.PeriodName, ", ",
                    period.PeriodName.Equals("Annuale", StringComparison.InvariantCultureIgnoreCase) ? entity.StartDate.Year.ToString() :
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(entity.StartDate.Month)));
            }

            entity.Year = Convert.ToInt16(entity.StartDate.Year);

            if (entity.Category != null)
            {
                int latestNumber = _unitOfWork.Repository<Fascicle>().GetLatestNumber(entity.Year, entity.Category.EntityShortId);
                entity.Number = ++latestNumber;

                IEnumerable<short> idCategories = entity.Category.FullIncrementalPath
                    .Split('|')
                    .Select(f => short.Parse(f));
                string categorySectionTitle = string.Join(".", _unitOfWork.Repository<Category>().Queryable(true)
                    .Where(f => idCategories.Contains(f.EntityShortId))
                    .OrderBy(f => f.EntityShortId)
                    .Select(f => f.Code));

                entity.Title = string.Concat(entity.Year, ".", categorySectionTitle, "-", entity.Number.ToString("0000000"));
            }

            FascicleFolder rootNode = new FascicleFolder
            {
                UniqueId = entity.UniqueId,
                Fascicle = entity,
                Name = string.Format("Tutti i documenti"),
                Category = entity.Category,
                Status = FascicleFolderStatus.Active,
                Typology = FascicleFolderTypology.Root,
            };
            _unitOfWork.Repository<FascicleFolder>().Insert(rootNode);
            if (entity.FascicleRoles != null)
            {
                HashSet<FascicleRole> fascicleRoles = new HashSet<FascicleRole>();
                FascicleRole fascicleRole;
                Role role;
                foreach (FascicleRole item in entity.FascicleRoles)
                {
                    fascicleRole = new FascicleRole();
                    role = null;
                    role = _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId);

                    if (role != null)
                    {
                        fascicleRole.Role = role;
                        fascicleRole.Fascicle = entity;
                        fascicleRole.IsMaster = item.IsMaster;

                        switch (entity.FascicleType)
                        {
                            case FascicleType.Activity:
                                {
                                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                                    break;
                                }
                            case FascicleType.Procedure:
                                {
                                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                                    if (item.IsMaster)
                                    {
                                        fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                                    }
                                    break;
                                }
                            case FascicleType.Period:
                                {
                                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                                    if (item.IsMaster)
                                    {
                                        fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                                    }
                                    break;
                                }
                        }
                        fascicleRoles.Add(fascicleRole);
                    }
                }
                entity.FascicleRoles = fascicleRoles;
                _unitOfWork.Repository<FascicleRole>().InsertRange(entity.FascicleRoles);
            }
            _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entity, FascicleLogType.Insert, "Inserimento nuovo fascicolo", CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<Fascicle> SetEntityIncludeOnUpdate(IQueryFluent<Fascicle> query)
        {
            if (!CurrentUpdateActionType.HasValue || (CurrentUpdateActionType.HasValue && CurrentUpdateActionType != UpdateActionType.OpenFascicleClosed))
            {
                query = query.Include(f => f.Contacts)
                    .Include(f => f.FascicleDocuments)
                    .Include(f => f.DossierFolders.Select(z => z.Dossier));
            }

            return query;
        }

        protected override bool ExecuteMappingBeforeUpdate()
        {
            return !CurrentUpdateActionType.HasValue || (CurrentUpdateActionType != UpdateActionType.OpenFascicleClosed && CurrentDeleteActionType != DeleteActionType.CancelFascicle);
        }

        protected override bool ExecuteDelete()
        {
            return CurrentDeleteActionType.HasValue && CurrentDeleteActionType != DeleteActionType.CancelFascicle || _needdelete;
        }

        protected override Fascicle BeforeUpdate(Fascicle entity, Fascicle entityTransformed)
        {

            if (entity.Contacts != null)
            {
                foreach (Contact item in entityTransformed.Contacts.Where(f => !entity.Contacts.Any(c => c.EntityId == f.EntityId)).ToList())
                {
                    entityTransformed.Contacts.Remove(item);
                    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Modify, string.Concat("Rimosso il contatto '", item.Description, "'"), CurrentDomainUser.Account));
                }
                foreach (Contact item in entity.Contacts.Where(f => !entityTransformed.Contacts.Any(c => c.EntityId == f.EntityId)))
                {
                    entityTransformed.Contacts.Add(_unitOfWork.Repository<Contact>().Find(item.EntityId));
                    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Modify, string.Concat("Aggiunto il contatto '", item.Description, "'"), CurrentDomainUser.Account));
                }
            }

            if (entity.Category != null)
            {
                entityTransformed.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.FascicleDocuments != null && entity.FascicleDocuments.Count > 0)
            {
                foreach (FascicleDocument item in entity.FascicleDocuments.Where(x => !entityTransformed.FascicleDocuments.Any(f => f.UniqueId == x.UniqueId)))
                {
                    _unitOfWork.Repository<FascicleDocument>().Insert(item);
                    entityTransformed.FascicleDocuments.Add(item);
                    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.DocumentInsert, string.Concat("Inserimento documento con id catena ", item.IdArchiveChain), CurrentDomainUser.Account));
                }
            }

            if (entityTransformed.DossierFolders != null)
            {
                DossierFolderStatus dossierFolderStatus = entity.EndDate.HasValue ? DossierFolderStatus.FascicleClose : DossierFolderStatus.Fascicle;
                bool changed = false;
                string dossierFolderName = string.Concat(entityTransformed.Title, "-", entityTransformed.FascicleObject);
                foreach (DossierFolder folder in entityTransformed.DossierFolders)
                {
                    changed = false;
                    if (folder.Status != dossierFolderStatus)
                    {
                        folder.Status = dossierFolderStatus;
                        changed = true;
                    }
                    if (folder.Name != dossierFolderName)
                    {
                        folder.Name = dossierFolderName;
                        changed = true;
                    }
                    if (changed)
                    {
                        _unitOfWork.Repository<DossierFolder>().Update(folder);
                        _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(folder.Dossier, folder, dossierFolderStatus == DossierFolderStatus.FascicleClose ? DossierLogType.FolderClose : DossierLogType.FolderModify,
                             string.Concat("La cartella ", folder.Name, dossierFolderStatus == DossierFolderStatus.FascicleClose ? " è stata chiusa dalla chiusura del fascicolo " : " è stata aggiornata dalla modifica dell'oggetto del fascicolo ", entity.Title, "-", entity.FascicleObject),
                             CurrentDomainUser.Account));
                    }
                }
            }

            string log = string.Concat(entity.EndDate.HasValue ? "Chiusura" : "Modifica", " del fascicolo");
            if (CurrentUpdateActionType.HasValue && CurrentUpdateActionType == UpdateActionType.OpenFascicleClosed)
            {
                entityTransformed.EndDate = null;
                log = "Riapertura del fascicolo";

            }
            _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, entity.EndDate.HasValue ? FascicleLogType.Close : FascicleLogType.Modify, log, CurrentDomainUser.Account));

            return base.BeforeUpdate(entity, entityTransformed);

        }

        protected override IQueryFluent<Fascicle> SetEntityIncludeOnDelete(IQueryFluent<Fascicle> query)
        {
            if (CurrentDeleteActionType.HasValue && CurrentDeleteActionType == DeleteActionType.CancelFascicle)
            {
                query = query
                    .Include(f => f.FascicleRoles)
                    .Include(f => f.FascicleDocuments)
                    .Include(f => f.FascicleLogs)
                    .Include(f => f.DossierFolders)
                    .Include(f => f.FascicleLinks)
                    .Include(f => f.WorkflowInstances)
                    .Include(f => f.Contacts);
            }

            return query;
        }

        protected override Fascicle BeforeDelete(Fascicle entity, Fascicle entityTransformed)
        {
            if (CurrentDeleteActionType.HasValue && CurrentDeleteActionType == DeleteActionType.CancelFascicle)
            {
                bool hasDocumentUnit = _unitOfWork.Repository<Fascicle>().HasDocumentUnits(entityTransformed.UniqueId);
                IQueryable<FascicleFolder> folders = _unitOfWork.Repository<FascicleFolder>().GetByIdFascicle(entityTransformed.UniqueId);

                if (!hasDocumentUnit && !entityTransformed.FascicleDocuments.Any())
                {
                    _needdelete = true;
                    entityTransformed.Contacts.Clear();
                    entityTransformed.WorkflowInstances.Clear();
                    entityTransformed.DossierFolders.Clear();
                    _unitOfWork.Repository<FascicleLink>().DeleteRange(entityTransformed.FascicleLinks);
                    entityTransformed.FascicleLinks.Clear();
                    _unitOfWork.Repository<FascicleLog>().DeleteRange(entityTransformed.FascicleLogs.ToList());
                    _unitOfWork.Repository<FascicleFolder>().DeleteRange(folders.ToList());
                    _unitOfWork.Repository<FascicleRole>().DeleteRange(entityTransformed.FascicleRoles.ToList());
                }

                if (hasDocumentUnit || entityTransformed.FascicleDocuments.Any())
                {
                    _needdelete = false;

                    IQueryable<FascicleDocumentUnit> fascicleDocumentUnits = _unitOfWork.Repository<FascicleDocumentUnit>().GetByFascicle(entityTransformed.UniqueId);

                    foreach (FascicleDocumentUnit documentUnit in fascicleDocumentUnits)
                    {
                        documentUnit.ReferenceType = ReferenceType.Reference;
                        _unitOfWork.Repository<FascicleDocumentUnit>().Update(documentUnit);
                    }

                    return entityTransformed;
                }

            }
            return base.BeforeDelete(entity, entityTransformed);
        }

        #endregion

    }
}

