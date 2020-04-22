using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class ContactService : BaseService<Contact>, IContactService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public ContactService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IContactRuleset ContactRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, ContactRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        protected override Contact BeforeCreate(Contact entity)
        {
            if (entity.Title != null)
            {
                entity.Title = _unitOfWork.Repository<ContactTitle>().Find(entity.Title.EntityId);
            }

            if (entity.PlaceName != null)
            {
                entity.PlaceName = _unitOfWork.Repository<ContactPlaceName>().Find(entity.PlaceName.EntityShortId);
            }

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<Contact> SetEntityIncludeOnUpdate(IQueryFluent<Contact> query)
        {
            return query.Include(i => i.Title)
                .Include(i => i.PlaceName);
        }

        protected override Contact BeforeUpdate(Contact entity, Contact entityTransformed)
        {
            if (entity.Title != null)
            {
                entityTransformed.Title = _unitOfWork.Repository<ContactTitle>().Find(entity.Title.EntityId);
            }

            if (entity.PlaceName != null)
            {
                entityTransformed.PlaceName = _unitOfWork.Repository<ContactPlaceName>().Find(entity.PlaceName.EntityShortId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}