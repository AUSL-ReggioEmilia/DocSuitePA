using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.JeepServiceHost;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.JeepServiceHosts
{
    public class JeepServiceHostService : BaseService<JeepServiceHost>, IJeepServiceHostService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public JeepServiceHostService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validatorService,
            IJeepServiceHostRuleset jeepServiceHostRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validatorService, jeepServiceHostRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
