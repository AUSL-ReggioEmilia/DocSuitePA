using VecompSoftware.DocSuiteWeb.Entity.Processes;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Processes
{
    public class ProcessFascicleWorkflowRepositoryMapper : BaseDomainMapper<ProcessFascicleWorkflowRepository, ProcessFascicleWorkflowRepository>, IProcessFascicleWorkflowRepositoryMapper
    {
        public ProcessFascicleWorkflowRepositoryMapper()
        {
        }

        public override ProcessFascicleWorkflowRepository Map(ProcessFascicleWorkflowRepository entity, ProcessFascicleWorkflowRepository entityTransformed)
        {
            #region [ Base ]

            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.ObjectState = entity.ObjectState;

            #endregion

            return entityTransformed;
        }
    }
}
