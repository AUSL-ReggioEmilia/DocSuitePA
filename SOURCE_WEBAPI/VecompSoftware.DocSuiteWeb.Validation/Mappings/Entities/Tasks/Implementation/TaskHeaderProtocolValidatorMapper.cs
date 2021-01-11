using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tasks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tasks
{
    public class TaskHeaderProtocolValidatorMapper : BaseMapper<TaskHeaderProtocol, TaskHeaderProtocolValidator>, ITaskHeaderProtocolValidatorMapper
    {
        public TaskHeaderProtocolValidatorMapper(){}

        public override TaskHeaderProtocolValidator Map(TaskHeaderProtocol entity, TaskHeaderProtocolValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.TaskHeader = entity.TaskHeader;
            entityTransformed.Protocol = entity.Protocol;
            #endregion

            return entityTransformed;
        }
    }
}
