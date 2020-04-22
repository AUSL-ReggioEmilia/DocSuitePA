using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Templates
{
    public class TemplateCollaborationService : BaseService<TemplateCollaboration>, ITemplateCollaborationService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]
        public TemplateCollaborationService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ITemplateCollaborationRuleset collaborationRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, collaborationRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override TemplateCollaboration BeforeCreate(TemplateCollaboration entity)
        {
            if (entity.TemplateCollaborationUsers != null && entity.TemplateCollaborationUsers.Count > 0)
            {
                foreach (TemplateCollaborationUser item in entity.TemplateCollaborationUsers)
                {
                    if (item.Role != null)
                    {
                        item.Role = _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId);
                    }
                }
                _unitOfWork.Repository<TemplateCollaborationUser>().InsertRange(entity.TemplateCollaborationUsers);
            }

            if (entity.TemplateCollaborationDocumentRepositories != null && entity.TemplateCollaborationDocumentRepositories.Count > 0)
            {
                foreach (TemplateCollaborationDocumentRepository item in entity.TemplateCollaborationDocumentRepositories)
                {
                    //TODO: Gestire entità templatedocumentrepositories
                }
                _unitOfWork.Repository<TemplateCollaborationDocumentRepository>().InsertRange(entity.TemplateCollaborationDocumentRepositories);
            }

            if (entity.Roles != null && entity.Roles.Count > 0)
            {
                entity.Roles = entity.Roles.Select(r => _unitOfWork.Repository<Role>().Find(r.EntityShortId)).ToList();
            }

            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, string.Concat("Inserimento template collaborazione", entity.Name), typeof(TemplateCollaboration).Name, CurrentDomainUser.Account));
            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<TemplateCollaboration> SetEntityIncludeOnUpdate(IQueryFluent<TemplateCollaboration> query)
        {
            query.Include(x => x.TemplateCollaborationUsers)
                .Include(x => x.Roles)
                .Include(x => x.TemplateCollaborationDocumentRepositories);
            return query;
        }
        protected override IQueryFluent<TemplateCollaboration> SetEntityIncludeOnDelete(IQueryFluent<TemplateCollaboration> query)
        {
            return query.Include(x => x.TemplateCollaborationUsers)
                .Include(x => x.Roles)
                .Include(x => x.TemplateCollaborationDocumentRepositories);
        }

        protected override TemplateCollaboration BeforeUpdate(TemplateCollaboration entity, TemplateCollaboration entityTransformed)
        {
            if (entity.TemplateCollaborationUsers != null)
            {
                foreach (TemplateCollaborationUser item in entity.TemplateCollaborationUsers.Where(f => entityTransformed.TemplateCollaborationUsers.Any(c => c.UniqueId == f.UniqueId)))
                {
                    TemplateCollaborationUser mappedUser = _mapperUnitOfWork.Repository<TemplateCollaborationUserMapper>().Map(item, entityTransformed.TemplateCollaborationUsers.First(c => c.UniqueId == item.UniqueId));
                    _unitOfWork.Repository<TemplateCollaborationUser>().Update(mappedUser);
                }
                foreach (TemplateCollaborationUser item in entityTransformed.TemplateCollaborationUsers.Where(f => !entity.TemplateCollaborationUsers.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    _unitOfWork.Repository<TemplateCollaborationUser>().Delete(item.UniqueId);
                }
                foreach (TemplateCollaborationUser item in entity.TemplateCollaborationUsers.Where(f => !entityTransformed.TemplateCollaborationUsers.Any(c => c.UniqueId == f.UniqueId)))
                {
                    item.TemplateCollaboration = entityTransformed;
                    if (item.Role != null)
                    {
                        item.Role = _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId);
                    }
                    _unitOfWork.Repository<TemplateCollaborationUser>().Insert(item);
                }
            }

            if (entity.TemplateCollaborationDocumentRepositories != null)
            {
                foreach (TemplateCollaborationDocumentRepository item in entityTransformed.TemplateCollaborationDocumentRepositories.Where(f => !entity.TemplateCollaborationDocumentRepositories.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    entityTransformed.TemplateCollaborationDocumentRepositories.Remove(item);
                }
                foreach (TemplateCollaborationDocumentRepository item in entity.TemplateCollaborationDocumentRepositories.Where(f => !entityTransformed.TemplateCollaborationDocumentRepositories.Any(c => c.UniqueId == f.UniqueId)))
                {
                    item.TemplateCollaboration = entityTransformed;
                    //TODO: Gestire entità templatedocumentrepositories
                    _unitOfWork.Repository<TemplateCollaborationDocumentRepository>().Insert(item);
                }
            }

            if (entity.Roles != null)
            {
                foreach (Role item in entityTransformed.Roles.Where(f => !entity.Roles.Any(c => c.EntityShortId == f.EntityShortId)).ToList())
                {
                    entityTransformed.Roles.Remove(item);
                }
                foreach (Role item in entity.Roles.Where(f => !entityTransformed.Roles.Any(c => c.EntityShortId == f.EntityShortId)))
                {
                    entityTransformed.Roles.Add(_unitOfWork.Repository<Role>().Find(item.EntityShortId));
                }
            }
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.UPDATE, string.Concat("Modificato template collaborazione", entity.Name), typeof(TemplateCollaboration).Name, CurrentDomainUser.Account));
            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override TemplateCollaboration BeforeDelete(TemplateCollaboration entity, TemplateCollaboration entityTransformed)
        {
            if (entityTransformed.TemplateCollaborationUsers != null)
            {
                _unitOfWork.Repository<TemplateCollaborationUser>().DeleteRange(entityTransformed.TemplateCollaborationUsers.ToList());
            }

            if (entityTransformed.TemplateCollaborationDocumentRepositories != null)
            {
                _unitOfWork.Repository<TemplateCollaborationDocumentRepository>().DeleteRange(entityTransformed.TemplateCollaborationDocumentRepositories.ToList());
            }

            if (entityTransformed.Roles != null)
            {
                entityTransformed.Roles.Clear();
            }

            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.DELETE, string.Concat("Eliminato template collaborazione", entity.Name), typeof(TemplateCollaboration).Name, CurrentDomainUser.Account));

            return base.BeforeDelete(entity, entityTransformed);
        }
        #endregion
    }
}
