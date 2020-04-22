using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.PECMails
{
    public class PECMailBoxConfigurationService : BaseService<PECMailBoxConfiguration>, IPECMailBoxConfigurationService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public PECMailBoxConfigurationService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validatorService,
            IPECMailBoxConfigurationRuleset pecMailBoxConfigurationRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validatorService, pecMailBoxConfigurationRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
