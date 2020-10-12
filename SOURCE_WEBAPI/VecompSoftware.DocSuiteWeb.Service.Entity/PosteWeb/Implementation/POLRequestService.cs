using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.PosteWeb
{
    public class POLRequestService : BaseService<PosteOnLineRequest>, IPOLRequestService
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public POLRequestService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IProcessRuleset processRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, processRuleset, mapperUnitOfWork, security)
        {
        }

        #endregion

        #region [ Methods ]

        protected override PosteOnLineRequest BeforeCreate(PosteOnLineRequest entity)
        {
            return base.BeforeCreate(entity);
        }

        protected override PosteOnLineRequest BeforeUpdate(PosteOnLineRequest entity, PosteOnLineRequest entityTransformed)
        {
            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion
    }
}
