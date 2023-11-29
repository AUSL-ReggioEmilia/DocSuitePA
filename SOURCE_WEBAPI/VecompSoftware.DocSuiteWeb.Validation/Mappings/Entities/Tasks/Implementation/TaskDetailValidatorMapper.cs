using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tasks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tasks
{
    public class TaskDetailValidatorMapper : BaseMapper<TaskDetail, TaskDetailValidator>, ITaskDetailValidatorMapper
    {
        public TaskDetailValidatorMapper(){}

        public override TaskDetailValidator Map(TaskDetail entity, TaskDetailValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Title = entity.Title;
            entityTransformed.Description = entity.Description;
            entityTransformed.ErrorDescription = entity.ErrorDescription;
            entityTransformed.DetailType = entity.DetailType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.TaskHeader = entity.TaskHeader;            
            #endregion

            return entityTransformed;
        }
    }
}
