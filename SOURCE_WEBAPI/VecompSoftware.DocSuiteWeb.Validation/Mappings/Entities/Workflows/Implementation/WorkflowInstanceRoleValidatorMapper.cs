using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows
{
    public class WorkflowInstanceRoleValidatorMapper : BaseMapper<WorkflowInstanceRole, WorkflowInstanceRoleValidator>, IWorkflowInstanceRoleValidatorMapper
    {
        public WorkflowInstanceRoleValidatorMapper() { }

        public override WorkflowInstanceRoleValidator Map(WorkflowInstanceRole entity, WorkflowInstanceRoleValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.AuthorizationType = entity.AuthorizationType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Role = entity.Role;
            entityTransformed.WorkflowInstance = entity.WorkflowInstance;
            #endregion

            return entityTransformed;
        }

    }
}
