using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tasks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tasks
{
    public class TaskHeaderValidatorMapper : BaseMapper<TaskHeader, TaskHeaderValidator>, ITaskHeaderValidatorMapper
    {
        public TaskHeaderValidatorMapper(){}

        public override TaskHeaderValidator Map(TaskHeader entity, TaskHeaderValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Code = entity.Code;
            entityTransformed.Title = entity.Title;
            entityTransformed.Description = entity.Description;
            entityTransformed.TaskType = entity.TaskType;
            entityTransformed.Status = entity.Status;
            entityTransformed.SendingProcessStatus = entity.SendingProcessStatus;
            entityTransformed.SendedStatus = entity.SendedStatus;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.TaskHeaderProtocols = entity.TaskHeaderProtocols;            
            #endregion

            return entityTransformed;
        }
    }
}
