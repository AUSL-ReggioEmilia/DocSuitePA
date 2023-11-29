using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.PECMails
{
    public class PECMailBoxService : BaseService<PECMailBox>, IPECMailBoxService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public PECMailBoxService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validatorService,
            IPECMailBoxRuleset pecMailBoxRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validatorService, pecMailBoxRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override IQueryFluent<PECMailBox> SetEntityIncludeOnUpdate(IQueryFluent<PECMailBox> query)
        {
            query.Include(d => d.Location);
            return query;
        }

        protected override PECMailBox BeforeUpdate(PECMailBox entity, PECMailBox entityTransformed)
        {
            if (entity.Location != null)
            {
                entityTransformed.Location = _unitOfWork.Repository<Location>().Find(entity.Location.EntityShortId);
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
