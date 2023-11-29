using VecompSoftware.DocSuiteWeb.Entity.Tasks;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Tasks
{
    public class TaskDetailMapper : BaseEntityMapper<TaskDetail, TaskDetail>, ITaskDetailMapper
    {
        public override TaskDetail Map(TaskDetail entity, TaskDetail entityTransformed)
        {
            #region [ Base ]
            entityTransformed.DetailType = entity.DetailType;
            entityTransformed.Title = entity.Title;
            entityTransformed.Description = entity.Description;
            entityTransformed.ErrorDescription = entity.ErrorDescription;
            #endregion

            return entityTransformed;
        }

    }
}
