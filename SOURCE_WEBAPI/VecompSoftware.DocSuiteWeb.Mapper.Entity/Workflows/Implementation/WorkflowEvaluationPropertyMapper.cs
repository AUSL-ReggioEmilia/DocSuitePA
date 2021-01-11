using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Workflows
{
    public class WorkflowEvaluationPropertyMapper : BaseEntityMapper<WorkflowEvaluationProperty, WorkflowEvaluationProperty>, IWorkflowEvaluationPropertyMapper
    {
        public override WorkflowEvaluationProperty Map(WorkflowEvaluationProperty entity, WorkflowEvaluationProperty entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Name = entity.Name;
            entityTransformed.WorkflowType = entity.WorkflowType;
            entityTransformed.PropertyType = entity.PropertyType;
            entityTransformed.ValueInt = entity.ValueInt;
            entityTransformed.ValueDate = entity.ValueDate;
            entityTransformed.ValueDouble = entity.ValueDouble;
            entityTransformed.ValueBoolean = entity.ValueBoolean;
            entityTransformed.ValueGuid = entity.ValueGuid;
            entityTransformed.ValueString = entity.ValueString;

            #endregion

            return entityTransformed;
        }
    }
}
