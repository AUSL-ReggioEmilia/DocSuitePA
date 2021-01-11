using VecompSoftware.DocSuiteWeb.Entity.Tasks;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Tasks
{
    public class TaskHeaderProtocolMapper : BaseEntityMapper<TaskHeaderProtocol, TaskHeaderProtocol>, ITaskHeaderProtocolMapper
    {
        public override TaskHeaderProtocol Map(TaskHeaderProtocol entity, TaskHeaderProtocol entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            #endregion

            return entityTransformed;
        }

    }
}
