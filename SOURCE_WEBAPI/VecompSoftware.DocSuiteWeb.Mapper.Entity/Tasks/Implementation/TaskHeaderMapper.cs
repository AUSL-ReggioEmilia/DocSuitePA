using VecompSoftware.DocSuiteWeb.Entity.Tasks;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Tasks
{
    public class TaskHeaderMapper : BaseEntityMapper<TaskHeader, TaskHeader>, ITaskHeaderMapper
    {
        public override TaskHeader Map(TaskHeader entity, TaskHeader entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Code = entity.Code;
            entityTransformed.Title = entity.Title;
            entityTransformed.Description = entity.Description;
            entityTransformed.TaskType = entity.TaskType;
            entityTransformed.Status = entity.Status;
            entityTransformed.SendingProcessStatus = entity.SendingProcessStatus;
            entityTransformed.SendedStatus = entity.SendedStatus;
            #endregion

            return entityTransformed;
        }

    }
}
