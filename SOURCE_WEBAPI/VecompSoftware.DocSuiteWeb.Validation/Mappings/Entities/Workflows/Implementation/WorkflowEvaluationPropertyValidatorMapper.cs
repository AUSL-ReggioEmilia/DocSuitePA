using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows
{
    public class WorkflowEvaluationPropertyValidatorMapper : BaseMapper<WorkflowEvaluationProperty, WorkflowEvaluationPropertyValidator>, IWorkflowEvaluationPropertyValidatorMapper
    {
        public WorkflowEvaluationPropertyValidatorMapper() { }

        public override WorkflowEvaluationPropertyValidator Map(WorkflowEvaluationProperty entity, WorkflowEvaluationPropertyValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.WorkflowType = entity.WorkflowType;
            entityTransformed.PropertyType = entity.PropertyType;
            entityTransformed.ValueInt = entity.ValueInt;
            entityTransformed.ValueDate = entity.ValueDate;
            entityTransformed.ValueDouble = entity.ValueDouble;
            entityTransformed.ValueBoolean = entity.ValueBoolean;
            entityTransformed.ValueGuid = entity.ValueGuid;
            entityTransformed.ValueString = entity.ValueString;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.WorkflowRepository = entity.WorkflowRepository;
            #endregion

            return entityTransformed;
        }
    }
}
