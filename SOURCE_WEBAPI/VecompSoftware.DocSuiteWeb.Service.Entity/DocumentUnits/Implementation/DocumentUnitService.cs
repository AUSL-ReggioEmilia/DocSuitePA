using System.Linq;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits
{
    public class DocumentUnitService : BaseService<DocumentUnit>, IDocumentUnitService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public DocumentUnitService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDocumentUnitRuleset documentUnitRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, documentUnitRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override DocumentUnit BeforeCreate(DocumentUnit entity)
        {
            if (entity.Category != null)
            {
                entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.Container != null)
            {
                entity.Container = _unitOfWork.Repository<Container>().Find(entity.Container.EntityShortId);
            }

            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            if (entity.UDSRepository != null)
            {
                entity.UDSRepository = _unitOfWork.Repository<UDSRepository>().Find(entity.UDSRepository.UniqueId);
            }

            if (entity.TenantAOO != null)
            {
                entity.TenantAOO = _unitOfWork.Repository<TenantAOO>().Find(entity.TenantAOO.UniqueId);
            }

            if (entity.DocumentUnitContacts != null && entity.DocumentUnitContacts.Count > 0)
            {
                foreach (DocumentUnitContact item in entity.DocumentUnitContacts)
                {
                    if (item.Contact != null && item.Contact.EntityId != 0)
                    {
                        item.Contact = _unitOfWork.Repository<Contact>().Find(item.Contact.EntityId);
                    }
                    item.DocumentUnit = entity;
                }
                _unitOfWork.Repository<DocumentUnitContact>().InsertRange(entity.DocumentUnitContacts);
            }

            if (entity.DocumentUnitChains != null && entity.DocumentUnitChains.Count > 0)
            {
                foreach (DocumentUnitChain item in entity.DocumentUnitChains)
                {
                    item.DocumentUnit = entity;
                }
                _unitOfWork.Repository<DocumentUnitChain>().InsertRange(entity.DocumentUnitChains);
            }
            
            if (entity.DocumentUnitRoles != null && entity.DocumentUnitRoles.Count > 0)
            {
                foreach (DocumentUnitRole item in entity.DocumentUnitRoles)
                {
                    item.DocumentUnit = entity;
                }
                _unitOfWork.Repository<DocumentUnitRole>().InsertRange(entity.DocumentUnitRoles);
            }
            
            if (entity.DocumentUnitUsers != null && entity.DocumentUnitUsers.Count > 0)
            {
                foreach (DocumentUnitUser item in entity.DocumentUnitUsers)
                {
                    item.DocumentUnit = entity;
                }
                _unitOfWork.Repository<DocumentUnitUser>().InsertRange(entity.DocumentUnitUsers);
            }

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<DocumentUnit> SetEntityIncludeOnUpdate(IQueryFluent<DocumentUnit> query)
        {
            query.Include(f => f.DocumentUnitChains)
                 .Include(f => f.DocumentUnitRoles)
                 .Include(f => f.DocumentUnitUsers)
                 .Include(f => f.DocumentUnitContacts)
                 .Include(f => f.Fascicle)
                 .Include(f => f.Category)
                 .Include(f => f.Container)
                 .Include(f => f.UDSRepository);
            return query;
        }

        protected override DocumentUnit BeforeUpdate(DocumentUnit entity, DocumentUnit entityTransformed)
        {
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Category = null;
            if (entity.Category != null)
            {
                entityTransformed.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            entityTransformed.Container = null;
            if (entity.Container != null)
            {
                entityTransformed.Container = _unitOfWork.Repository<Container>().Find(entity.Container.EntityShortId);
            }

            entityTransformed.Fascicle = null;
            if (entity.Fascicle != null)
            {
                entityTransformed.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            entityTransformed.UDSRepository = null;
            if (entity.UDSRepository != null)
            {
                entityTransformed.UDSRepository = _unitOfWork.Repository<UDSRepository>().Find(entity.UDSRepository.UniqueId);
            }

            if (entity.DocumentUnitChains != null)
            {
                foreach (DocumentUnitChain item in entityTransformed.DocumentUnitChains.Where(f => !entity.DocumentUnitChains.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    if (item.ChainType == ChainType.DematerialisationChain || item.ChainType == ChainType.MetadataChain)
                    {
                        _logger.WriteInfo(new LogMessage($"DocumentUnitService:User={CurrentDomainUser.Account}:Skipping deletion of DocumentUnitChain with ChainType={item.ChainType}, UniqueId={item.UniqueId}, IdArchiveChain={item.IdArchiveChain}"), LogCategories);
                        continue;
                    }
                    _unitOfWork.Repository<DocumentUnitChain>().Delete(item);
                }
                foreach (DocumentUnitChain item in entity.DocumentUnitChains.Where(f => !entityTransformed.DocumentUnitChains.Any(c => c.UniqueId == f.UniqueId)))
                {
                    item.DocumentUnit = entityTransformed;
                    _unitOfWork.Repository<DocumentUnitChain>().Insert(item);
                }
            }

            if (entity.DocumentUnitRoles != null)
            {

                foreach (DocumentUnitRole item in entityTransformed.DocumentUnitRoles.Where(f => !entity.DocumentUnitRoles.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    _unitOfWork.Repository<DocumentUnitRole>().Delete(item);
                }
                //Aggiorno i settori esistenti con il RoleAuthorizationType modificato
                foreach (DocumentUnitRole item in entityTransformed.DocumentUnitRoles.Where(f => entity.DocumentUnitRoles.Any(c => c.UniqueId == f.UniqueId && c.AuthorizationRoleType != f.AuthorizationRoleType)).ToList())
                {
                    item.AuthorizationRoleType = entity.DocumentUnitRoles.First(d => d.UniqueId == item.UniqueId).AuthorizationRoleType;
                    _unitOfWork.Repository<DocumentUnitRole>().Update(item);
                }
                foreach (DocumentUnitRole item in entity.DocumentUnitRoles.Where(f => !entityTransformed.DocumentUnitRoles.Any(c => c.UniqueId == f.UniqueId)))
                {
                    item.DocumentUnit = entityTransformed;
                    _unitOfWork.Repository<DocumentUnitRole>().Insert(item);
                }
            }

            if (entity.DocumentUnitUsers != null)
            {
                foreach (DocumentUnitUser item in entityTransformed.DocumentUnitUsers.Where(f => !entity.DocumentUnitUsers.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    _unitOfWork.Repository<DocumentUnitUser>().Delete(item);
                }
                foreach (DocumentUnitUser item in entity.DocumentUnitUsers.Where(f => !entityTransformed.DocumentUnitUsers.Any(c => c.UniqueId == f.UniqueId)))
                {
                    item.DocumentUnit = entityTransformed;
                    _unitOfWork.Repository<DocumentUnitUser>().Insert(item);
                }
            }

            if (entity.DocumentUnitContacts != null)
            {
                foreach (DocumentUnitContact resolutionDocumentUnitContact in entityTransformed.DocumentUnitContacts.Where(f => !entity.DocumentUnitContacts.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    _unitOfWork.Repository<DocumentUnitContact>().Delete(resolutionDocumentUnitContact);
                }

                foreach (DocumentUnitContact resolutionDocumentUnitContact in entity.DocumentUnitContacts.Where(f => !entityTransformed.DocumentUnitContacts.Any(c => c.UniqueId == f.UniqueId)))
                {
                    if (resolutionDocumentUnitContact.Contact != null && resolutionDocumentUnitContact.Contact.EntityId != 0)
                    {
                        resolutionDocumentUnitContact.Contact = _unitOfWork.Repository<Contact>().Find(resolutionDocumentUnitContact.Contact.EntityId);
                    }
                    resolutionDocumentUnitContact.DocumentUnit = entityTransformed;
                    _unitOfWork.Repository<DocumentUnitContact>().Insert(resolutionDocumentUnitContact);
                }
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion

    }
}
