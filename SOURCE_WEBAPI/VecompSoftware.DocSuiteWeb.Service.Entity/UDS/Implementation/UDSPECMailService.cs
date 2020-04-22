using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public class UDSPECMailService : BaseService<UDSPECMail>, IUDSPECMailService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public UDSPECMailService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validatorService,
            IUDSRuleset udsRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validatorService, udsRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override UDSPECMail BeforeCreate(UDSPECMail entity)
        {
            if (entity.Repository != null)
            {
                entity.Repository = _unitOfWork.Repository<UDSRepository>().Find(entity.Repository.UniqueId);
            }

            if (entity.Relation != null)
            {
                entity.Relation = _unitOfWork.Repository<PECMail>().Find(entity.Relation.EntityId);
            }

            return base.BeforeCreate(entity);
        }

        protected override UDSPECMail BeforeUpdate(UDSPECMail entity, UDSPECMail entityTransformed)
        {

            if (entity.Repository != null)
            {
                entityTransformed.Repository = _unitOfWork.Repository<UDSRepository>().Find(entity.Repository.UniqueId);
            }

            if (entity.Relation != null)
            {
                entityTransformed.Relation = _unitOfWork.Repository<PECMail>().Find(entity.Relation.EntityId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
