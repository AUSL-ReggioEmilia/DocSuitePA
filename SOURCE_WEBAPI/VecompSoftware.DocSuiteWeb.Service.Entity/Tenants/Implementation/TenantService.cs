using System.Collections.Generic;
using System.Linq;
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
            if(entity.Contacts != null)
            {
                HashSet<Contact> tenantContacts = new HashSet<Contact>();
                Contact tenantContact = null;
                foreach(Contact item in entity.Contacts)
                {
                    tenantContact = _unitOfWork.Repository<Contact>().Find(item.EntityId);
                    if(tenantContact != null)
                    {
                        tenantContacts.Add(tenantContact);
                    }
                }
                entity.Contacts = tenantContacts;
            }

            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, string.Concat("Inserimento azienda ", entity.TenantName), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, string.Concat("Inserimento azienda ", entity.TenantName), typeof(Contact).Name, CurrentDomainUser.Account));

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
            if (entity.Configurations != null)
            {
                foreach (TenantConfiguration item in entityTransformed.Configurations.Where(t => !entity.Configurations.Any(c => c.UniqueId == t.UniqueId)).ToList())
                {
                    _unitOfWork.Repository<TenantConfiguration>().Delete(item);
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimossa la configurazione '", item.Tenant != null ? item.Tenant.TenantName : "", "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                }

                foreach (TenantConfiguration item in entity.Configurations.Where(t => !entityTransformed.Configurations.Any(c => c.UniqueId == t.UniqueId)))
                {
                    item.Tenant = entityTransformed;
                    _unitOfWork.Repository<TenantConfiguration>().Insert(item);
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunta la configurazione '", item.Tenant != null ? item.Tenant.TenantName : "", "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                }
            }

            #region Contacts
            if (entity.Contacts != null)
            {
                foreach (Contact item in entityTransformed.Contacts.Where(f => !entity.Contacts.Any(c => c.EntityId == f.EntityId)).ToList())
                {
                    entityTransformed.Contacts.Remove(item);
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimosso il contatto '", item.Description, "'"), typeof(Contact).Name, CurrentDomainUser.Account));
                }
                foreach (Contact item in entity.Contacts.Where(f => !entityTransformed.Contacts.Any(c => c.EntityId == f.EntityId)))
                {
                    entityTransformed.Contacts.Add(_unitOfWork.Repository<Contact>().Find(item.EntityId));
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunto il contatto '", item.Description, "'"), typeof(Contact).Name, CurrentDomainUser.Account));
                }
            }
            #endregion

            #region Containers
            if (entity.Containers != null)
            {
                foreach (Container item in entityTransformed.Containers.Where(f => !entity.Containers.Any(c => c.EntityShortId == f.EntityShortId)).ToList())
                {
                    entityTransformed.Containers.Remove(item);
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimosso il contenitore '", item.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                }
                foreach (Container item in entity.Containers.Where(f => !entityTransformed.Containers.Any(c => c.EntityShortId == f.EntityShortId)))
                {
                    entityTransformed.Containers.Add(_unitOfWork.Repository<Container>().Find(item.EntityShortId));
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunto il contenitore '", item.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                }
            }
            #endregion

            #region Roles
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
            #endregion

            #region PECMailBoxes
            if (entity.PECMailBoxes != null)
            {
                foreach (PECMailBox item in entityTransformed.PECMailBoxes.Where(f => !entity.PECMailBoxes.Any(c => c.EntityShortId == f.EntityShortId)).ToList())
                {
                    entityTransformed.PECMailBoxes.Remove(item);
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimosso la casella PEC '", item.MailBoxRecipient, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                }
                foreach (PECMailBox item in entity.PECMailBoxes.Where(f => !entityTransformed.PECMailBoxes.Any(c => c.EntityShortId == f.EntityShortId)))
                {
                    entityTransformed.PECMailBoxes.Add(_unitOfWork.Repository<PECMailBox>().Find(item.EntityShortId));
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunto la casella PEC '", item.MailBoxRecipient, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                }
            }
            #endregion

            #region WorkflowRepositories
            if (entity.TenantWorkflowRepositories != null)
            {
                foreach (TenantWorkflowRepository item in entityTransformed.TenantWorkflowRepositories.Where(f => !entity.TenantWorkflowRepositories.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    entityTransformed.TenantWorkflowRepositories.Remove(item);
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Rimosso il flusso di lavoro '", item.WorkflowRepository.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                }
                foreach (TenantWorkflowRepository item in entity.TenantWorkflowRepositories.Where(f => !entityTransformed.TenantWorkflowRepositories.Any(c => c.UniqueId == f.UniqueId)))
                {
                    entityTransformed.TenantWorkflowRepositories.Add(_unitOfWork.Repository<TenantWorkflowRepository>().Find(item.UniqueId));
                    _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.INSERT, string.Concat("Aggiunto il flusso di lavoro '", item.WorkflowRepository.Name, "'"), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));
                }
            }
            #endregion


            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion

    }
}
