using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Service.Workflow;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Workflows;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Workflow
{
    [LogCategory(LogCategoryDefinition.WEBAPIWORKFLOW)]
    public class WorkflowValidateController : BaseWorkflowController<WorkflowValidate, IWorkflowValidateService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;

        #endregion

        #region [ Constructor ]
        public WorkflowValidateController(IWorkflowValidateService service, IValidatorService validationService, IWorkflowRuleset ruleset, ILogger logger)
            : base(service, validationService, ruleset, logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]
        #endregion
    }
}