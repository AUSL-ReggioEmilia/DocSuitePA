using VecompSoftware.DocSuiteWeb.Entity.Processes;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Processes
{
    public class ProcessMapper : BaseEntityMapper<Process, Process>, IProcessMapper
    {
        public ProcessMapper()
        {
        }

        public override Process Map(Process entity, Process entityTransformed)
        {
            #region [ Base ]

            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.ObjectState = entity.ObjectState;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Name = entity.Name;
            entityTransformed.Note = entity.Note;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.ProcessType = entity.ProcessType;

            #endregion

            return entityTransformed;
        }
    }
}
