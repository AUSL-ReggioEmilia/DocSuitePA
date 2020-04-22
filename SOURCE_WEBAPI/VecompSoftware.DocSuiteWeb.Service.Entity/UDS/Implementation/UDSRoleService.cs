using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public class UDSRoleService : BaseUDSService<UDSRole>, IUDSRoleService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public UDSRoleService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IUDSRuleset ruleset, IMapperUnitOfWork mapper, ISecurity security)
            : base(unitOfWork, logger, validationService, ruleset, mapper, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]

        protected override string GetLogMessage(UDSRole entity)
        {
            return $"autorizzazione: {entity.Relation.EntityShortId.ToString()} nell'archivio: {entity.Repository.Name}";
        }

        #endregion
    }
}
