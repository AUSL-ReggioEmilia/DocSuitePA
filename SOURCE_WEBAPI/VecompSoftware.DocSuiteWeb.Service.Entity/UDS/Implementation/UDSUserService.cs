using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public class UDSUserService : BaseUDSService<UDSUser>, IUDSUserService
    {
        #region [ Constructor ]
        public UDSUserService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IUDSRuleset ruleset, IMapperUnitOfWork mapper, ISecurity security)
            : base(unitOfWork, logger, validationService, ruleset, mapper, security)
        {
        }
        #endregion

        #region [ Methods ]

        protected override string GetLogMessage(UDSUser entity)
        {
            return $"autorizzazione all'utente: {entity.Account} nell'archivio: {entity.Repository.Name}";
        }
        #endregion
    }
}
