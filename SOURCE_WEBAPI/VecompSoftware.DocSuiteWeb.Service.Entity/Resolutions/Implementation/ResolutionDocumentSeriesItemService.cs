using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Resolutions
{
    public class ResolutionDocumentSeriesItemService : BaseService<ResolutionDocumentSeriesItem>, IResolutionDocumentSeriesItemService
    {

        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public ResolutionDocumentSeriesItemService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService, IResolutionRuleset validatorRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security) 
            : base(unitOfWork, logger, validationService, validatorRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        #endregion

    }
}
