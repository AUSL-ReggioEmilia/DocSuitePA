using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers
{
    public class DossierService : BaseDossierService<Dossier>, IDossierService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private readonly IParameterEnvService _parameterEnvService;
        #endregion

        #region [ Constructor ]

        public DossierService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDossierRuleset dossierRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security, IParameterEnvService parameterEnvService)
            : base(unitOfWork, logger, validationService, dossierRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _mapperUnitOfWork = mapperUnitOfWork;
            _parameterEnvService = parameterEnvService;
        }
        #endregion

        #region [ Methods ]

        protected override Dossier BeforeCreate(Dossier entity)
        {
            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entity, null, DossierLogType.Insert, string.Concat("Nuovo dossier '", entity.Subject, "'"), CurrentDomainUser.Account));

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

            if (entity.MetadataRepository != null)
            {
                entity.MetadataRepository = _unitOfWork.Repository<MetadataRepository>().Find(entity.MetadataRepository.UniqueId);
            }

            if (entity.Container != null)
            {
                entity.Container = _unitOfWork.Repository<Container>().Find(entity.Container.EntityShortId);
            }

            if (entity.DossierRoles != null)
            {
                HashSet<DossierRole> dossierRoles = new HashSet<DossierRole>();
                foreach (DossierRole item in entity.DossierRoles)
                {
                    dossierRoles.Add(CreateDossierRole(item, entity));
                }
                entity.DossierRoles = dossierRoles.Where(f => f != null).ToList();
                _unitOfWork.Repository<DossierRole>().InsertRange(entity.DossierRoles);
            }

            if (entity.DossierDocuments != null && entity.DossierDocuments.Count > 0)
            {
                _unitOfWork.Repository<DossierDocument>().InsertRange(entity.DossierDocuments);
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
                    metadataValue.Dossier = entity;
                    metadataValues.Add(metadataValue);
                }
                entity.SourceMetadataValues = metadataValues;
                entity.MetadataValueContacts = metadataContactValues;
                _unitOfWork.Repository<MetadataValue>().InsertRange(entity.SourceMetadataValues);
                _unitOfWork.Repository<MetadataValueContact>().InsertRange(entity.MetadataValueContacts);
            }

            if (entity.Category != null)
            {
                entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }
            else
            {
                UserLog userLog = _unitOfWork.Repository<UserLog>().GetBySystemUser(CurrentDomainUser.Account);
                Tenant currentTenant = _unitOfWork.Repository<Tenant>().GetByUniqueId(userLog.CurrentTenantId).FirstOrDefault();
                entity.Category = _unitOfWork.Repository<Category>().GetDefaultCategoryByTenantAOOId(currentTenant.TenantAOO.UniqueId);
            }

            entity.Status = DossierStatus.Open;

            entity.Year = Convert.ToInt16(DateTime.UtcNow.Year);
            entity.Number = -1;
            int latestNumber = _unitOfWork.Repository<Dossier>().GetLatestNumber(entity.Year);
            entity.Number = ++latestNumber;

            DossierFolder rootNode = new DossierFolder
            {
                UniqueId = entity.UniqueId,
                Dossier = entity,
                Name = $"Dossier {entity.Year}/{entity.Number:0000000}"
                ,
                Status = DossierFolderStatus.InProgress,
            };

            _unitOfWork.Repository<DossierFolder>().Insert(rootNode);
            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entity, rootNode, DossierLogType.FolderInsert, string.Concat("Creata cartella radice '", rootNode.Name, "'"), CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        private DossierRole CreateDossierRole(DossierRole entity, Dossier dossier)
        {
            DossierRole dossierRole = null;
            if (entity.Role == null)
            {
                return dossierRole;
            }
            Role role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);

            if (role != null)
            {
                dossierRole = new DossierRole
                {
                    Role = role,
                    Dossier = dossier,
                    AuthorizationRoleType = entity.AuthorizationRoleType,
                    IsMaster = entity.IsMaster,
                    Status = DossierRoleStatus.Active
                };
                _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(dossier, null, DossierLogType.Authorize,
                    string.Concat("Autorizzato il dossier al settore '", role.Name, "' (", role.EntityShortId, ") responsabile (Responsible)"), CurrentDomainUser.Account));
            }

            return dossierRole;
        }

        private MetadataValueContact CreateMetadataContactValue(MetadataValueModel metadataValueModel, Dossier dossier)
        {
            MetadataValueContact metadataContactValue = new MetadataValueContact();
            metadataContactValue.Dossier = dossier;
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

        protected override IQueryFluent<Dossier> SetEntityIncludeOnUpdate(IQueryFluent<Dossier> query)
        {
            query.Include(d => d.Contacts)
                 .Include(d => d.DossierDocuments)
                 .Include(d => d.SourceMetadataValues)
                 .Include(d => d.MetadataValueContacts);
            return query;
        }

        protected override Dossier BeforeUpdate(Dossier entity, Dossier entityTransformed)
        {
            if (entity.Contacts != null)
            {
                foreach (Contact item in entityTransformed.Contacts.Where(f => !entity.Contacts.Any(c => c.EntityId == f.EntityId)).ToList())
                {
                    entityTransformed.Contacts.Remove(item);
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.Modify, string.Concat("Rimosso il contatto '", item.Description, "'"), CurrentDomainUser.Account));
                }
                foreach (Contact item in entity.Contacts.Where(f => !entityTransformed.Contacts.Any(c => c.EntityId == f.EntityId)))
                {
                    entityTransformed.Contacts.Add(_unitOfWork.Repository<Contact>().Find(item.EntityId));
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.Modify, string.Concat("Aggiunto il contatto '", item.Description, "'"), CurrentDomainUser.Account));
                }
            }

            if (entity.DossierDocuments != null && entity.DossierDocuments.Any())
            {
                foreach (DossierDocument item in entity.DossierDocuments.Where(x => !entityTransformed.DossierDocuments.Any(f => f.UniqueId == x.UniqueId)))
                {
                    _unitOfWork.Repository<DossierDocument>().Insert(item);
                    entityTransformed.DossierDocuments.Add(item);
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.DocumentInsert, string.Concat("Inserimento documento con id catena ", item.IdArchiveChain), CurrentDomainUser.Account));
                }
            }
            if (entity.Container != null)
            {
                entityTransformed.Container = _unitOfWork.Repository<Container>().Find(entity.Container.EntityShortId);
            }

            if (entityTransformed.SourceMetadataValues != null && entityTransformed.MetadataValueContacts != null && !string.IsNullOrEmpty(entity.MetadataValues))
            {
                ICollection<MetadataValueModel> metadataValueModels = JsonConvert.DeserializeObject<ICollection<MetadataValueModel>>(entity.MetadataValues);
                MetadataDesignerModel metadataDesignerModel = JsonConvert.DeserializeObject<MetadataDesignerModel>(entityTransformed.MetadataDesigner);
                foreach (MetadataValue item in entityTransformed.SourceMetadataValues.Where(c => !metadataValueModels.Any(x => x.KeyName == c.Name)).ToList())
                {
                    _unitOfWork.Repository<MetadataValue>().Delete(item);
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.Modify,
                        string.Concat("Rimosso il metadato '", item.Name, "'"), CurrentDomainUser.Account));
                }
                foreach (MetadataValueContact item in entityTransformed.MetadataValueContacts.Where(c => !metadataValueModels.Any(x => x.KeyName == c.Name)).ToList())
                {
                    _unitOfWork.Repository<MetadataValueContact>().Delete(item);
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.Modify,
                        string.Concat("Rimosso il metadato '", item.Name, "'"), CurrentDomainUser.Account));
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
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.Modify,
                        string.Concat("Creato il metadato '", metadataValueModel.KeyName, "'"), CurrentDomainUser.Account));
                }
                foreach (MetadataValueModel metadataValueModel in metadataValueModels.Where(x => !metadataDesignerModel.ContactFields.Any(xx => xx.KeyName == x.KeyName)
                                && !entityTransformed.SourceMetadataValues.Any(xx => xx.Name == x.KeyName)))
                {
                    tmpMetadataValue = MetadataValueService.CreateMetadataValue(metadataDesignerModel, metadataValueModel);
                    tmpMetadataValue.Dossier = entityTransformed;
                    _unitOfWork.Repository<MetadataValue>().Insert(tmpMetadataValue);
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.Modify,
                        string.Concat("Creato il metadato '", metadataValueModel.KeyName, "'"), CurrentDomainUser.Account));
                }
            }

            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, entity.EndDate.HasValue ? DossierLogType.Close : DossierLogType.Modify,
                string.Concat(entity.EndDate.HasValue ? "Chiusura" : "Modifica", " del Dossier"), CurrentDomainUser.Account));

            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion
    }
}