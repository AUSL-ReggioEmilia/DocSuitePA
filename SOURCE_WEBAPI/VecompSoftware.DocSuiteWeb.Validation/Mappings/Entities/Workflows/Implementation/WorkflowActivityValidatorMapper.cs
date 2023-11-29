using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows
{
    public class WorkflowActivityValidatorMapper : BaseMapper<WorkflowActivity, WorkflowActivityValidator>, IWorkflowActivityValidatorMapper
    {
        public WorkflowActivityValidatorMapper() { }

        public override WorkflowActivityValidator Map(WorkflowActivity entity, WorkflowActivityValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.DueDate = entity.DueDate;
            entityTransformed.Name = entity.Name;
            entityTransformed.ActivityType = entity.ActivityType;
            entityTransformed.ActivityAction = entity.ActivityAction;
            entityTransformed.ActivityArea = entity.ActivityArea;
            entityTransformed.Status = entity.Status;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            entityTransformed.Priority = entity.Priority;
            entityTransformed.Note = entity.Note;
            entityTransformed.IsVisible = entity.IsVisible;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.WorkflowActivityLogs = entity.WorkflowActivityLogs;
            entityTransformed.WorkflowInstance = entity.WorkflowInstance;
            entityTransformed.WorkflowProperties = entity.WorkflowProperties;
            entityTransformed.WorkflowAuthorizations = entity.WorkflowAuthorizations;
            entityTransformed.DocumentUnitReferenced = entity.DocumentUnitReferenced;
            entityTransformed.Tenant = entity.Tenant;

            #endregion

            return entityTransformed;
        }
    }
}
