using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations
{
    public class CollaborationDraftService : BaseService<CollaborationDraft>, ICollaborationDraftService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public CollaborationDraftService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ICollaborationRuleset conservationRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, conservationRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        protected override CollaborationDraft BeforeCreate(CollaborationDraft entity)
        {
            if (entity.Collaboration != null)
            {
                entity.Collaboration = _unitOfWork.Repository<Collaboration>().Find(entity.Collaboration.EntityId);
            }
            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<CollaborationDraft> SetEntityIncludeOnUpdate(IQueryFluent<CollaborationDraft> query)
        {
            query.Include(f => f.Collaboration);
            return query;
        }

        protected override CollaborationDraft BeforeUpdate(CollaborationDraft entity, CollaborationDraft entityTransformed)
        {
            if (entity.Collaboration != null)
            {
                entityTransformed.Collaboration = _unitOfWork.Repository<Collaboration>().Find(entity.Collaboration.EntityId);
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion

    }
}
