using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class IncrementalService : BaseService<Incremental>, IIncrementalService
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IncrementalService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IIncrementalRuleset categoryRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, categoryRuleset, mapperUnitOfWork, security)
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}