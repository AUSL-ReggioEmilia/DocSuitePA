using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Workflows
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsWorkflowAuthorized : BaseValidator<WorkflowActivity, WorkflowActivityValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructior]
        public IsWorkflowAuthorized(NameValueCollection attributes)
            : base("The WorkflowActivityValidator is null", nameof(IsWorkflowAuthorized))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(WorkflowActivityValidator objectToValidate)
        {
            DomainUserModel currentUser = objectToValidate.CurrentSecurity?.GetCurrentUser();
            bool res = CurrentUnitOfWork.Repository<WorkflowActivity>().IsAuthorized(currentUser.Name, currentUser.Domain, objectToValidate.UniqueId);
            if (!res)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
