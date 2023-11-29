using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Workflows
{
    public class WorkflowActivityMapper : BaseEntityMapper<WorkflowActivity, WorkflowActivity>, IWorkflowActivityMapper
    {
        public override WorkflowActivity Map(WorkflowActivity entity, WorkflowActivity entityTransformed)
        {
            //entityTransformed.DueDate = entity.DueDate;
            //entityTransformed.Name = entity.Name;
            //entityTransformed.ActivityType = entity.ActivityType;
            //entityTransformed.ActivityAction = entity.ActivityAction;
            //entityTransformed.ActivityArea = entity.ActivityArea;
            //entityTransformed.Status = entity.Status;
            //entityTransformed.Subject = entity.Subject;
            //entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            //entityTransformed.Priority = entity.Priority;
            entityTransformed.Note = entity.Note;
            //entityTransformed.ActivityArea = entity.ActivityArea;
            //entityTransformed.ActivityAction = entity.ActivityAction;
            //entityTransformed.IsVisible = entity.IsVisible;
            return entityTransformed;
        }

    }
}
