using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Tenants
{
    public class TenantService : BaseService<Tenant>, ITenantService
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]

        public TenantService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ITenantRuleset tenantRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, tenantRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override Tenant BeforeCreate(Tenant entity)
        {
            if (entity.TenantAOO != null)
            {
                entity.TenantAOO = _unitOfWork.Repository<TenantAOO>().Find(entity.TenantAOO.UniqueId);
            }

            if (entity.Configurations != null && entity.Configurations.Count > 0)
            {
                foreach (TenantConfiguration item in entity.Configurations)
                {
                    item.Tenant = entity;
                }
            }

            if (entity.Containers != null)
            {
                HashSet<Container> containers = new HashSet<Container>();
                Container container = null;
                foreach (Container item in entity.Containers)
                {
                    container = _unitOfWork.Repository<Container>().Find(item.EntityShortId);
                    if (container != null)
                    {
                        containers.Add(container);
                    }
                }
                entity.Containers = containers;
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

            if (entity.PECMailBoxes != null)
            {
                HashSet<PECMailBox> pecMailBoxes = new HashSet<PECMailBox>();
                PECMailBox pecMailBox = null;
                foreach (PECMailBox item in entity.PECMailBoxes)
                {
                    pecMailBox = _unitOfWork.Repository<PECMailBox>().Find(item.EntityShortId);
                    if (pecMailBox != null)
                    {
                        pecMailBoxes.Add(pecMailBox);
                    }
                }
                entity.PECMailBoxes = pecMailBoxes;
            }

            if (entity.TenantWorkflowRepositories != null)
            {
                HashSet<TenantWorkflowRepository> tenantWorkflowRepositories = new HashSet<TenantWorkflowRepository>();
                TenantWorkflowRepository tenantWorkflowRepository = null;
                foreach (TenantWorkflowRepository item in entity.TenantWorkflowRepositories)
                {
                    tenantWorkflowRepository = _unitOfWork.Repository<TenantWorkflowRepository>().Find(item.UniqueId);
                    if (tenantWorkflowRepository != null)
                    {
                        tenantWorkflowRepositories.Add(tenantWorkflowRepository);
                    }
                }
                entity.TenantWorkflowRepositories = tenantWorkflowRepositories;
            }

            if (entity.Contacts != null)
            {
                HashSet<Contact> tenantContacts = new HashSet<Contact>();
                Contact tenantContact = null;
                foreach (Contact item in entity.Contacts)
                {
                    tenantContact = _unitOfWork.Repository<Contact>().Find(item.EntityId);
                    if (tenantContact != null)
                    {
                        tenantContacts.Add(tenantContact);
                    }
                }
                entity.Contacts = tenantContacts;
            }

            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, string.Concat("Inserimento OU ", entity.TenantName), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<Tenant> SetEntityIncludeOnUpdate(IQueryFluent<Tenant> query)
        {
            query.Include(d => d.Configurations)
                .Include(d => d.Containers)
                .Include(d => d.Roles)
                .Include(d => d.PECMailBoxes)
                .Include(d => d.TenantWorkflowRepositories)
                .Include(d => d.Contacts);
            return query;
        }

        protected override Tenant BeforeUpdate(Tenant entity, Tenant entityTransformed)
        {
            if (CurrentUpdateActionType.HasValue)
            {
                switch (CurrentUpdateActionType.Value)
                {
                    case UpdateActionType.TenantConfigurationAdd:
                        {
                            TenantConfiguration configuration = entity.Configurations.FirstOrDefault();
                            configuration.Tenant = entityTransformed;
                            _unitOfWork.Repository<TenantConfiguration>().Insert(configuration);
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunta la configurazione '", configuration.Tenant != null ? configuration.Tenant.TenantName : "", "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantConfigurationRemove:
                        {
                            TenantConfiguration configuration = entityTransformed.Configurations.FirstOrDefault(x=>x.UniqueId == entity.Configurations.FirstOrDefault().UniqueId);
                            _unitOfWork.Repository<TenantConfiguration>().Delete(configuration);
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimossa la configurazione '", configuration.Tenant != null ? configuration.Tenant.TenantName : "", "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantContactAdd:
                        {
                            Contact contact = entity.Contacts.FirstOrDefault();
                            entityTransformed.Contacts.Add(_unitOfWork.Repository<Contact>().Find(contact.EntityId));
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunto il contatto '", contact.Description, "'"), typeof(Contact).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantContactRemove:
                        {
                            Contact contact = entityTransformed.Contacts.FirstOrDefault(x=>x.EntityId == entity.Contacts.FirstOrDefault().EntityId);
                            entityTransformed.Contacts.Remove(contact);
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimosso il contatto '", contact.Description, "'"), typeof(Contact).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantContainerAdd:
                        {
                            Container container = entity.Containers.FirstOrDefault();
                            entityTransformed.Containers.Add(_unitOfWork.Repository<Container>().Find(container.EntityShortId));
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunto il contenitore '", container.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantContainerRemove:
                        {
                            Container container = entityTransformed.Containers.FirstOrDefault(x=>x.EntityShortId == entity.Containers.FirstOrDefault().EntityShortId);
                            entityTransformed.Containers.Remove(container);
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimosso il contenitore '", container.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantRoleAdd:
                        {
                            foreach (Role role in entity.Roles)
                            {
                                entityTransformed.Roles.Add(_unitOfWork.Repository<Role>().Find(role.EntityShortId));
                                _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunto il settore '", role.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                            }
                            break;
                        }
                    case UpdateActionType.TenantRoleRemove:
                        {
                            Role role = entityTransformed.Roles.FirstOrDefault(x => x.EntityShortId == entity.Roles.FirstOrDefault().EntityShortId);
                            entityTransformed.Roles.Remove(role);
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimosso il settore '", role.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantPECMailBoxAdd:
                        {
                            PECMailBox pecMailBox = entity.PECMailBoxes.FirstOrDefault();
                            entityTransformed.PECMailBoxes.Add(_unitOfWork.Repository<PECMailBox>().Find(pecMailBox.EntityShortId));
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunto la casella PEC '", pecMailBox.MailBoxRecipient, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantPECMailBoxRemove:
                        {
                            PECMailBox pecMailBox = entityTransformed.PECMailBoxes.FirstOrDefault(x=>x.EntityShortId == entity.PECMailBoxes.FirstOrDefault().EntityShortId);
                            entityTransformed.PECMailBoxes.Remove(pecMailBox);
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimosso la casella PEC '", pecMailBox.MailBoxRecipient, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantWorkflowRepositoryAdd:
                        {
                            TenantWorkflowRepository tenantWorkflowRepository = entity.TenantWorkflowRepositories.FirstOrDefault();
                            entityTransformed.TenantWorkflowRepositories.Add(_unitOfWork.Repository<TenantWorkflowRepository>().Find(tenantWorkflowRepository.UniqueId));
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunto il flusso di lavoro '", tenantWorkflowRepository.WorkflowRepository.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantWorkflowRepositoryRemove:
                        {
                            TenantWorkflowRepository tenantWorkflowRepository = entityTransformed.TenantWorkflowRepositories.FirstOrDefault(x=>x.UniqueId == entity.TenantWorkflowRepositories.FirstOrDefault().UniqueId);
                            entityTransformed.TenantWorkflowRepositories.Remove(tenantWorkflowRepository);
                            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimosso il flusso di lavoro '", tenantWorkflowRepository.WorkflowRepository.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                            break;
                        }
                    case UpdateActionType.TenantContainerAddAll:
                        {
                            entityTransformed.Containers = _unitOfWork.Repository<Container>().Queryable(true).ToList();
                            break;
                        }
                    case UpdateActionType.TenantContainerRemoveAll:
                        {
                            entityTransformed.Containers.Clear();
                            break;
                        }
                    case UpdateActionType.TenantRoleAddAll:
                        {
                            entityTransformed.Roles = _unitOfWork.Repository<Role>().Queryable(true).ToList();
                            break;
                        }
                    case UpdateActionType.TenantRoleRemoveAll:
                        {
                            entityTransformed.Roles.Clear();
                            break;
                        }
                    case UpdateActionType.TenantContactAddAll:
                        {
                            entityTransformed.Contacts = _unitOfWork.Repository<Contact>().Queryable(true).ToList();
                            break;
                        }
                    case UpdateActionType.TenantContactRemoveAll:
                        {
                            entityTransformed.Contacts.Clear();
                            break;
                        }
                }
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion

    }
}
