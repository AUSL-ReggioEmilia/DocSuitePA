using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows
{
    public class WorkflowAuthorizationValidatorMapper : BaseMapper<WorkflowAuthorization, WorkflowAuthorizationValidator>, IWorkflowAuthorizationValidatorMapper
    {
        public WorkflowAuthorizationValidatorMapper() { }

        public override WorkflowAuthorizationValidator Map(WorkflowAuthorization entity, WorkflowAuthorizationValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Account = entity.Account;
            entityTransformed.IsHandler = entity.IsHandler;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.WorkflowActivity = entity.WorkflowActivity;
            #endregion

            return entityTransformed;
        }

    }
}
