using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Workflow;

namespace VecompSoftware.DocSuiteWeb.Mapper.Workflow
{
    public class WorkflowArgumentMapper : BaseWorkflowMapper<WorkflowArgument, WorkflowProperty>, IWorkflowArgumentMapper
    {
        public override WorkflowProperty Map(WorkflowArgument entity, WorkflowProperty entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.PropertyType = (WorkflowPropertyType)((short)entity.PropertyType);
            entityTransformed.ValueBoolean = entity.ValueBoolean;
            entityTransformed.ValueDate = entity.ValueDate;
            entityTransformed.ValueDouble = entity.ValueDouble;
            entityTransformed.ValueGuid = entity.ValueGuid;
            entityTransformed.ValueInt = entity.ValueInt;
            entityTransformed.ValueString = entity.ValueString;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }

    }
}
