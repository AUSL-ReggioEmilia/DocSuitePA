using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives
{
    public class DocumentSeriesItemLogService : BaseService<DocumentSeriesItemLog>, IDocumentSeriesItemLogService
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DocumentSeriesItemLogService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDocumentSeriesRuleset documentSeriesRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, documentSeriesRuleset, mapperUnitOfWork, security)
        {

        }
        #endregion
    }
}
