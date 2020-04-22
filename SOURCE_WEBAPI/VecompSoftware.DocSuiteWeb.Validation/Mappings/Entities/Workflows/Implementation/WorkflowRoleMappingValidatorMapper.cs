using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows
{
    public class WorkflowRoleMappingValidatorMapper : BaseMapper<WorkflowRoleMapping, WorkflowRoleMappingValidator>, IWorkflowRoleMappingValidatorMapper
    {
        public WorkflowRoleMappingValidatorMapper() { }

        public override WorkflowRoleMappingValidator Map(WorkflowRoleMapping entity, WorkflowRoleMappingValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.AuthorizationType = entity.AuthorizationType;
            entityTransformed.MappingTag = entity.MappingTag;
            entityTransformed.IdInternalActivity = entity.IdInternalActivity;
            entityTransformed.AccountName = entity.AccountName;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]

            entityTransformed.WorkflowRepository = entity.WorkflowRepository;
            entityTransformed.Role = entity.Role;




            #endregion

            return entityTransformed;
        }

    }
}
