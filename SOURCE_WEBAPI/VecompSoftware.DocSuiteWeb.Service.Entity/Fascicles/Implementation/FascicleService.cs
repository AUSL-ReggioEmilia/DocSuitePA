using Newtonsoft.Json;
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
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
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
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private bool _needdelete = true;
        #endregion

        #region [ Constructor ]

        public FascicleService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IFascicleRuleset fascicleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, fascicleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapperUnitOfWork = mapperUnitOfWork;
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
            fascicleLog.Hash = HashGenerator.GenerateHash($"{fascicleLog.RegistrationUser}|{fascicleLog.LogType}|{fascicleLog.LogDescription}|{fascicleLog.UniqueId}|{fascicle.UniqueId}|{fascicleLog.RegistrationDate:yyyyMMddHHmmss}");
            return fascicleLog;
        }

        protected override Fascicle BeforeCreate(Fascicle entity)
        {
            entity.FascicleFolders.Clear();
            if (entity.Contacts != null)
            {
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

            if (entity.FascicleTemplate != null)
            {
                entity.FascicleTemplate = _unitOfWork.Repository<ProcessFascicleTemplate>().Find(entity.FascicleTemplate.UniqueId);
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
                    .Where(f => f.Code > 0)
                    .Select(f => f.Code));

                entity.Title = $"{entity.Year}.{categorySectionTitle}-{entity.Number:0000000}";
            }

            if (entity.DossierFolders != null)
            {
                HashSet<DossierFolder> dossierFolders = new HashSet<DossierFolder>();
                DossierFolder dossierFolderParent = null;
                DossierFolder dossierFascicleFolder = null;
                foreach (DossierFolder dossierFolder in entity.DossierFolders)
                {
                    dossierFolderParent = _unitOfWork.Repository<DossierFolder>().GetIncludeDossier(dossierFolder.UniqueId);
                    if (dossierFolderParent != null)
                    {
                        dossierFolderParent.Status = DossierFolderStatus.Folder;
                        _unitOfWork.Repository<DossierFolder>().Update(dossierFolderParent);
                        dossierFascicleFolder = new DossierFolder()
                        {
                            UniqueId = Guid.NewGuid(),
                            Fascicle = entity,
                            Dossier = dossierFolderParent.Dossier,
                            ParentInsertId = dossierFolderParent.UniqueId,
                            Name = $"{entity.Title}-{entity.FascicleObject}",
                            Status = DossierFolderStatus.Fascicle,
                            Category = entity.Category
                        };
                        dossierFolders.Add(dossierFascicleFolder);
                    }

                    if (dossierFolderParent.Dossier.DossierType == DossierType.Process)
                    {
                        entity.DossierFolderLabel = _unitOfWork.Repository<Fascicle>().GetAllDossierFolderLabelName(dossierFolder.UniqueId);
                        entity.ProcessLabel = dossierFolderParent.Dossier.Processes.FirstOrDefault()?.Name;
                    }
                }
                entity.DossierFolders = dossierFolders;
                _unitOfWork.Repository<DossierFolder>().InsertRange(entity.DossierFolders);
            }

            if (entity.FascicleRoles != null)
            {
                HashSet<FascicleRole> fascicleRoles = new HashSet<FascicleRole>();
                FascicleRole fascicleRole;
                Role role;
                foreach (FascicleRole item in entity.FascicleRoles)
                {
                    fascicleRole = new FascicleRole();
                    role = _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId);
                    if (role != null && !fascicleRoles.Any(f => f.Role.EntityShortId == role.EntityShortId))
                    {
                        fascicleRole.Role = role;
                        fascicleRole.Fascicle = entity;
                        fascicleRole.IsMaster = item.IsMaster;
                        fascicleRole.AuthorizationRoleType = item.AuthorizationRoleType;

                        switch (entity.FascicleType)
                        {
                            case FascicleType.Activity:
                                {
                                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                                    break;
                                }
                            case FascicleType.Procedure:
                            case FascicleType.Period:
                                {
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

            if (!string.IsNullOrEmpty(entity.MetadataValues))
            {
                ICollection<MetadataValueModel> metadataValueModels = JsonConvert.DeserializeObject<ICollection<MetadataValueModel>>(entity.MetadataValues);
                MetadataDesignerModel metadataDesignerModel = JsonConvert.DeserializeObject<MetadataDesignerModel>(entity.MetadataDesigner);
                ICollection<MetadataValue> metadataValues = new List<MetadataValue>();
                ICollection<MetadataValueContact> metadataContactValues = new List<MetadataValueContact>();
                foreach (MetadataValueModel metadataValueModel in metadataValueModels.Where(x => metadataDesignerModel.ContactFields.Any(xx => xx.KeyName == x.KeyName)))
                {
                    metadataContactValues.Add(CreateMetadataContactValue(metadataValueModel, entity));
                }

                MetadataValue metadataValue;
                foreach (MetadataValueModel metadataValueModel in metadataValueModels.Where(x => !metadataDesignerModel.ContactFields.Any(xx => xx.KeyName == x.KeyName)))
                {
                    metadataValue = MetadataValueService.CreateMetadataValue(metadataDesignerModel, metadataValueModel);
                    metadataValue.Fascicle = entity;
                    metadataValues.Add(metadataValue);
                }
                entity.SourceMetadataValues = metadataValues;
                entity.MetadataValueContacts = metadataContactValues;
                _unitOfWork.Repository<MetadataValue>().InsertRange(entity.SourceMetadataValues);
                _unitOfWork.Repository<MetadataValueContact>().InsertRange(entity.MetadataValueContacts);
            }

            _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entity, FascicleLogType.Insert, "Inserimento nuovo fascicolo", CurrentDomainUser.Account));

            _unitOfWork.Repository<FascicleFolder>().Insert(new FascicleFolder
            {
                UniqueId = entity.UniqueId,
                Fascicle = entity,
                Name = "Tutti i documenti",
                Category = null,
                Status = FascicleFolderStatus.Active,
                Typology = FascicleFolderTypology.Root
            });
            return base.BeforeCreate(entity);
        }

        private MetadataValueContact CreateMetadataContactValue(MetadataValueModel metadataValueModel, Fascicle fascicle)
        {
            MetadataValueContact metadataContactValue = new MetadataValueContact();
            metadataContactValue.Fascicle = fascicle;
            metadataContactValue.Name = metadataValueModel.KeyName;
            ContactModel contactModel = JsonConvert.DeserializeObject<ContactModel>(metadataValueModel.Value);
            if (contactModel.Id.HasValue)
            {
                metadataContactValue.Contact = _unitOfWork.Repository<Contact>().Find(contactModel.Id.Value);
            }
            else
            {
                metadataContactValue.ContactManual = metadataValueModel.Value;
            }
            return metadataContactValue;
        }

        protected override IQueryFluent<Fascicle> SetEntityIncludeOnUpdate(IQueryFluent<Fascicle> query)
        {
            if (!CurrentUpdateActionType.HasValue || (CurrentUpdateActionType.HasValue && CurrentUpdateActionType != UpdateActionType.OpenFascicleClosed))
            {
                query = query.Include(f => f.Contacts)
                    .Include(f => f.FascicleDocuments)
                    .Include(f => f.DossierFolders.Select(z => z.Dossier))
                    .Include(f => f.SourceMetadataValues)
                    .Include(f => f.MetadataValueContacts);
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
            if (entity.Category != null)
            {
                entityTransformed.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.Contacts != null && entity.Contacts.Any())
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

            if (entity.FascicleDocuments != null && entity.FascicleDocuments.Count > 0)
            {
                foreach (FascicleDocument item in entity.FascicleDocuments.Where(x => !entityTransformed.FascicleDocuments.Any(f => f.UniqueId == x.UniqueId)))
                {
                    _unitOfWork.Repository<FascicleDocument>().Insert(item);
                    entityTransformed.FascicleDocuments.Add(item);
                    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.DocumentInsert, string.Concat("Inserimento documento con id catena ", item.IdArchiveChain), CurrentDomainUser.Account));
                }
            }

            if (entityTransformed.SourceMetadataValues != null && entityTransformed.MetadataValueContacts != null && !string.IsNullOrEmpty(entity.MetadataValues))
            {
                ICollection<MetadataValueModel> metadataValueModels = JsonConvert.DeserializeObject<ICollection<MetadataValueModel>>(entity.MetadataValues);
                MetadataDesignerModel metadataDesignerModel = JsonConvert.DeserializeObject<MetadataDesignerModel>(entityTransformed.MetadataDesigner);
                foreach (MetadataValue item in entityTransformed.SourceMetadataValues.Where(c => !metadataValueModels.Any(x => x.KeyName == c.Name)).ToList())
                {
                    _unitOfWork.Repository<MetadataValue>().Delete(item);
                    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Modify, string.Concat("Rimosso il metadato '", item.Name, "'"), CurrentDomainUser.Account));
                }
                foreach (MetadataValueContact item in entityTransformed.MetadataValueContacts.Where(c => !metadataValueModels.Any(x => x.KeyName == c.Name)).ToList())
                {
                    _unitOfWork.Repository<MetadataValueContact>().Delete(item);
                    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Modify, string.Concat("Rimosso il metadato '", item.Name, "'"), CurrentDomainUser.Account));
                }

                MetadataValueContact tmpContactValue;
                foreach (MetadataValueContact item in entityTransformed.MetadataValueContacts.Where(c => metadataValueModels.Any(x => x.KeyName == c.Name)).ToList())
                {
                    tmpContactValue = CreateMetadataContactValue(metadataValueModels.Single(x => x.KeyName == item.Name), entityTransformed);
                    item.Contact = tmpContactValue.Contact;
                    item.ContactManual = tmpContactValue.ContactManual;
                    _unitOfWork.Repository<MetadataValueContact>().Update(item);
                }
                MetadataValue tmpMetadataValue;
                foreach (MetadataValue item in entityTransformed.SourceMetadataValues.Where(c => metadataValueModels.Any(x => x.KeyName == c.Name)).ToList())
                {
                    tmpMetadataValue = MetadataValueService.CreateMetadataValue(metadataDesignerModel, metadataValueModels.Single(x => x.KeyName == item.Name));
                    _mapperUnitOfWork.Repository<IDomainMapper<MetadataValue, MetadataValue>>().Map(tmpMetadataValue, item);
                    _unitOfWork.Repository<MetadataValue>().Update(item);
                }

                foreach (MetadataValueModel metadataValueModel in metadataValueModels.Where(x => metadataDesignerModel.ContactFields.Any(xx => xx.KeyName == x.KeyName)
                                && !entityTransformed.MetadataValueContacts.Any(xx => xx.Name == x.KeyName)))
                {
                    _unitOfWork.Repository<MetadataValueContact>().Insert(CreateMetadataContactValue(metadataValueModel, entityTransformed));
                    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Modify, string.Concat("Creato il metadato '", metadataValueModel.KeyName, "'"), CurrentDomainUser.Account));
                }
                foreach (MetadataValueModel metadataValueModel in metadataValueModels.Where(x => !metadataDesignerModel.ContactFields.Any(xx => xx.KeyName == x.KeyName)
                                && !entityTransformed.SourceMetadataValues.Any(xx => xx.Name == x.KeyName)))
                {
                    tmpMetadataValue = MetadataValueService.CreateMetadataValue(metadataDesignerModel, metadataValueModel);
                    tmpMetadataValue.Fascicle = entityTransformed;
                    _unitOfWork.Repository<MetadataValue>().Insert(tmpMetadataValue);
                    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Modify, string.Concat("Creato il metadato '", metadataValueModel.KeyName, "'"), CurrentDomainUser.Account));
                }
            }

            if (CurrentUpdateActionType == UpdateActionType.AssociatedProcessDossierFolderToFascicle)
            {
                InsertProcessDossierFolder(entity, entityTransformed);
            }
            else
            {
                UpdateFascicleDossierFolders(entity, entityTransformed);
            }

            string log = string.Concat(entity.EndDate.HasValue ? "Chiusura" : "Modifica", " del fascicolo");
            if (CurrentUpdateActionType.HasValue && CurrentUpdateActionType == UpdateActionType.OpenFascicleClosed)
            {
                entityTransformed.EndDate = null;
                log = "Riapertura del fascicolo";

            }
            _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, entity.EndDate.HasValue ? FascicleLogType.Close : FascicleLogType.Modify, log, CurrentDomainUser.Account));

            if (CurrentUpdateActionType == UpdateActionType.ChangeFascicleType)
            {
                entityTransformed.FascicleType = FascicleType.Procedure;
                _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Modify, string.Concat("Trasforma in fascicolo di procedimento."), CurrentDomainUser.Account));
            }
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
                    .Include(f => f.Contacts)
                    .Include(f => f.SourceMetadataValues)
                    .Include(f => f.MetadataValueContacts);
            }

            return query;
        }

        protected override Fascicle BeforeDelete(Fascicle entity, Fascicle entityTransformed)
        {
            if (CurrentDeleteActionType.HasValue && CurrentDeleteActionType == DeleteActionType.CancelFascicle)
            {
                bool hasDocumentUnit = _unitOfWork.Repository<Fascicle>().HasDocumentUnits(entityTransformed.UniqueId);
                IQueryable<FascicleFolder> folders = _unitOfWork.Repository<FascicleFolder>().GetByIdFascicle(entityTransformed.UniqueId);

                //if (!hasDocumentUnit && !entityTransformed.FascicleDocuments.Any())
                //{
                //    _needdelete = true;
                //    entityTransformed.Contacts.Clear();
                //    entityTransformed.WorkflowInstances.Clear();
                //    entityTransformed.DossierFolders.Clear();
                //    _unitOfWork.Repository<FascicleLink>().DeleteRange(entityTransformed.FascicleLinks);
                //    entityTransformed.FascicleLinks.Clear();
                //    _unitOfWork.Repository<FascicleLog>().DeleteRange(entityTransformed.FascicleLogs.ToList());
                //    _unitOfWork.Repository<FascicleFolder>().DeleteRange(folders.ToList());
                //    _unitOfWork.Repository<FascicleRole>().DeleteRange(entityTransformed.FascicleRoles.ToList());
                //    _unitOfWork.Repository<MetadataValue>().DeleteRange(entityTransformed.SourceMetadataValues.ToList());
                //    _unitOfWork.Repository<MetadataValueContact>().DeleteRange(entityTransformed.MetadataValueContacts.ToList());
                //}

                //if (hasDocumentUnit || entityTransformed.FascicleDocuments.Any())
                //{
                //    _needdelete = false;

                //    IQueryable<FascicleDocumentUnit> fascicleDocumentUnits = _unitOfWork.Repository<FascicleDocumentUnit>().GetByFascicle(entityTransformed.UniqueId);

                //    foreach (FascicleDocumentUnit documentUnit in fascicleDocumentUnits)
                //    {
                //        documentUnit.ReferenceType = ReferenceType.Reference;
                //        _unitOfWork.Repository<FascicleDocumentUnit>().Update(documentUnit);
                //    }
                //    entityTransformed.EndDate = DateTimeOffset.UtcNow;
                //    _unitOfWork.Repository<Fascicle>().Update(entityTransformed);
                //    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Close, "Fascicolo annullato", CurrentDomainUser.Account));
                //    return entityTransformed;
                //}

                _needdelete = false;

                IQueryable<FascicleDocumentUnit> fascicleDocumentUnits = _unitOfWork.Repository<FascicleDocumentUnit>().GetByFascicle(entityTransformed.UniqueId);

                foreach (FascicleDocumentUnit documentUnit in fascicleDocumentUnits)
                {
                    documentUnit.ReferenceType = ReferenceType.Reference;
                    _unitOfWork.Repository<FascicleDocumentUnit>().Update(documentUnit);
                }
                entityTransformed.EndDate = DateTimeOffset.UtcNow;
                _unitOfWork.Repository<Fascicle>().Update(entityTransformed);
                _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Close, "Fascicolo annullato", CurrentDomainUser.Account));
                return entityTransformed;
            }
            return base.BeforeDelete(entity, entityTransformed);
        }

        private void InsertProcessDossierFolder(Fascicle entity, Fascicle entityTransformed)
        {
            if (entity.DossierFolders == null || entity.DossierFolders.Any(df => entityTransformed.DossierFolders.Any(d => d.UniqueId == df.UniqueId)))
            {
                return;
            }

            HashSet<DossierFolder> dossierFolders = new HashSet<DossierFolder>();
            DossierFolder dossierFolderParent = null;
            DossierFolder dossierFascicleFolder = null;
            foreach (DossierFolder dossierFolder in entity.DossierFolders)
            {
                dossierFolderParent = _unitOfWork.Repository<DossierFolder>().GetIncludeDossier(dossierFolder.UniqueId);
                if (dossierFolderParent != null)
                {
                    dossierFolderParent.Status = DossierFolderStatus.Folder;
                    _unitOfWork.Repository<DossierFolder>().Update(dossierFolderParent);
                    dossierFascicleFolder = new DossierFolder()
                    {
                        UniqueId = Guid.NewGuid(),
                        Fascicle = entityTransformed,
                        Dossier = dossierFolderParent.Dossier,
                        ParentInsertId = dossierFolderParent.UniqueId,
                        Name = $"{entityTransformed.Title}-{entityTransformed.FascicleObject}",
                        Status = DossierFolderStatus.Fascicle,
                        Category = entityTransformed.Category
                    };
                    dossierFolders.Add(dossierFascicleFolder);

                    if (dossierFolderParent.Dossier.DossierType == DossierType.Process)
                    {
                        entityTransformed.DossierFolderLabel = _unitOfWork.Repository<Fascicle>().GetAllDossierFolderLabelName(dossierFolder.UniqueId);
                        entityTransformed.ProcessLabel = dossierFolderParent.Dossier.Processes.FirstOrDefault()?.Name;
                    }
                }
            }
            DossierFolder oldDossierFolder = entityTransformed.DossierFolders.FirstOrDefault();
            entityTransformed.DossierFolders = dossierFolders;
            _unitOfWork.Repository<DossierFolder>().InsertRange(entityTransformed.DossierFolders);
            if (oldDossierFolder != null && oldDossierFolder.Status == DossierFolderStatus.Fascicle)
            {
                _unitOfWork.Repository<DossierFolder>().Delete(oldDossierFolder);
            }
        }

        private void UpdateFascicleDossierFolders(Fascicle entity, Fascicle entityTransformed)
        {
            if (entity.DossierFolders != null && entity.DossierFolders.Count > 0)
            {
                foreach (DossierFolder dossierFolder in entityTransformed.DossierFolders.Where(df => !entity.DossierFolders.Any(d => d.UniqueId == df.UniqueId)).ToList())
                {
                    entityTransformed.DossierFolders.Remove(dossierFolder);
                    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Modify, $"Rimosso il dossier '{dossierFolder.Name }'", CurrentDomainUser.Account));
                }

                foreach (DossierFolder dossierFolder in entity.DossierFolders.Where(df => !entityTransformed.DossierFolders.Any(d => d.UniqueId == df.UniqueId)).ToList())
                {
                    entityTransformed.DossierFolders.Add(_unitOfWork.Repository<DossierFolder>().GetIncludeDossier(dossierFolder.UniqueId));
                    _unitOfWork.Repository<FascicleLog>().Insert(CreateLog(entityTransformed, FascicleLogType.Modify, $"Aggiunto il dossier '{dossierFolder.Name }'", CurrentDomainUser.Account));
                }
            }

            if (entityTransformed.DossierFolders != null)
            {
                DossierFolderStatus dossierFolderStatus = entity.EndDate.HasValue ? DossierFolderStatus.FascicleClose : DossierFolderStatus.Fascicle;
                bool changed = false;
                string dossierFolderName = $"{entityTransformed.Title}-{entityTransformed.FascicleObject}";
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
                             $"La cartella {folder.Name} {(dossierFolderStatus == DossierFolderStatus.FascicleClose ? " è stata chiusa dalla chiusura del fascicolo " : " è stata aggiornata dalla modifica dell'oggetto del fascicolo")} {entity.Title}-{entity.FascicleObject}",
                             CurrentDomainUser.Account));
                    }
                    if (folder.Dossier.DossierType == DossierType.Process)
                    {
                        DossierFolder dossierFolderParent = _unitOfWork.Repository<DossierFolder>().GetIncludeDossier(folder.UniqueId, optimization: true);
                        entityTransformed.DossierFolderLabel = _unitOfWork.Repository<Fascicle>().GetAllDossierFolderLabelName(folder.UniqueId);
                        entityTransformed.ProcessLabel =dossierFolderParent.Dossier.Processes.FirstOrDefault()?.Name;
                    }
                }
            }
        }

        #endregion

    }
}

