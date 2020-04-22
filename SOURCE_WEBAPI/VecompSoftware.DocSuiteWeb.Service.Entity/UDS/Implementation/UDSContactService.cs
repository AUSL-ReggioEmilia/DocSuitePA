using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public class UDSContactService : BaseUDSService<UDSContact>, IUDSContactService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public UDSContactService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationervice,
            IUDSRuleset ruleset, IMapperUnitOfWork mapper, ISecurity security)
            : base(unitOfWork, logger, validationervice, ruleset, mapper, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override UDSContact BeforeCreate(UDSContact entity)
        {
            if (entity.Repository != null)
            {
                entity.Repository = _unitOfWork.Repository<UDSRepository>().Find(entity.Repository.UniqueId);
            }

            if (entity.Relation != null)
            {
                entity.Relation = _unitOfWork.Repository<Contact>().Find(entity.Relation.EntityId);
            }

            return base.BeforeCreate(entity);
        }

        protected override UDSContact BeforeUpdate(UDSContact entity, UDSContact entityTransformed)
        {

            if (entity.Repository != null)
            {
                entityTransformed.Repository = _unitOfWork.Repository<UDSRepository>().Find(entity.Repository.UniqueId);
            }

            if (entity.Relation != null)
            {
                entityTransformed.Relation = _unitOfWork.Repository<Contact>().Find(entity.Relation.EntityId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion
    }
}