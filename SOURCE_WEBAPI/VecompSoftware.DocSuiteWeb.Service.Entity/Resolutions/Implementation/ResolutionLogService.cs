using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Resolutions
{
    public class ResolutionLogService : BaseService<ResolutionLog>, IResolutionLogService
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public ResolutionLogService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validatorService, IResolutionRuleset resolutionRuleset,
            IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validatorService, resolutionRuleset, mapperUnitOfWork, security)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
