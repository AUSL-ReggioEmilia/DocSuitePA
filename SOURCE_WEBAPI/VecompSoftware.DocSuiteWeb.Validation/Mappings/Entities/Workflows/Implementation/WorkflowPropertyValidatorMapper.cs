using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows
{
    public class WorkflowPropertyValidatorMapper : BaseMapper<WorkflowProperty, WorkflowPropertyValidator>, IWorkflowPropertyValidatorMapper
    {
        public WorkflowPropertyValidatorMapper() { }

        public override WorkflowPropertyValidator Map(WorkflowProperty entity, WorkflowPropertyValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.PropertyType = entity.PropertyType;
            entityTransformed.ValueBoolean = entity.ValueBoolean;
            entityTransformed.ValueDate = entity.ValueDate;
            entityTransformed.ValueDouble = entity.ValueDouble;
            entityTransformed.ValueInt = entity.ValueInt;
            entityTransformed.ValueString = entity.ValueString;
            entityTransformed.WorkflowType = entity.WorkflowType;
            entityTransformed.PropertyType = entity.PropertyType;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.WorkflowInstance = entity.WorkflowInstance;
            entityTransformed.WorkflowActivity = entity.WorkflowActivity;
            #endregion

            return entityTransformed;
        }

    }
}
