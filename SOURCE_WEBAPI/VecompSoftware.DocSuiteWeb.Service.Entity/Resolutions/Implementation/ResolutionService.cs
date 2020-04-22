using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Resolutions
{
    public class ResolutionService : BaseService<Resolution>, IResolutionService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public ResolutionService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IResolutionRuleset resolutionRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, resolutionRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override IQueryFluent<Resolution> SetEntityIncludeOnUpdate(IQueryFluent<Resolution> query)
        {
            query.Include(i => i.FileResolution);
            return query;
        }

        protected override Resolution BeforeUpdate(Resolution entity, Resolution entityTransformed)
        {
            if (CurrentUpdateActionType.HasValue && CurrentUpdateActionType.Value == UpdateActionType.CompleteDematerialisationWorkflow && entityTransformed.FileResolution != null)
            {
                entityTransformed.FileResolution.DematerialisationChainId = entity.FileResolution.DematerialisationChainId;
            }
            return entityTransformed;
        }


        #endregion
    }
}
