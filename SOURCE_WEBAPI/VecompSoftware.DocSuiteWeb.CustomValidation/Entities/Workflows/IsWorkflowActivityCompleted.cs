using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Workflows
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsWorkflowActivityCompleted : BaseValidator<WorkflowActivity, WorkflowActivityValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor]
        public IsWorkflowActivityCompleted(NameValueCollection attributes)
            : base("The WorkflowActivityValidator is null", nameof(IsWorkflowActivityCompleted))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(WorkflowActivityValidator objectToValidate)
        {
            WorkflowActivity fullActivity = CurrentUnitOfWork.Repository<WorkflowActivity>().GetByUniqueId(objectToValidate.UniqueId);

            if (fullActivity != null && fullActivity.Status == WorkflowStatus.Done)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
