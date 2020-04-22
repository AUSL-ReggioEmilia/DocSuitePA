using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Protocols
{
    public class ProtocolDraftService : BaseService<ProtocolDraft>, IProtocolDraftService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public ProtocolDraftService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IProtocolRuleset protocolRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, protocolRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        protected override ProtocolDraft BeforeCreate(ProtocolDraft entity)
        {
            if (entity.Collaboration != null)
            {
                entity.Collaboration = _unitOfWork.Repository<Collaboration>().Find(entity.Collaboration.EntityId);
            }
            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<ProtocolDraft> SetEntityIncludeOnUpdate(IQueryFluent<ProtocolDraft> query)
        {
            query.Include(f => f.Collaboration);
            return query;
        }

        protected override ProtocolDraft BeforeUpdate(ProtocolDraft entity, ProtocolDraft entityTransformed)
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
