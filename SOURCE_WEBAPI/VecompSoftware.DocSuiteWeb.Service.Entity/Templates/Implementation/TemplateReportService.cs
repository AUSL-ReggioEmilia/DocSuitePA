using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Templates
{
    public class TemplateReportService : BaseService<TemplateReport>, ITemplateReportService
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public TemplateReportService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ITemplateDocumentRepositoryRuleset templateDocumentRepositoryRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, templateDocumentRepositoryRuleset, mapperUnitOfWork, security)
        {

        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}
