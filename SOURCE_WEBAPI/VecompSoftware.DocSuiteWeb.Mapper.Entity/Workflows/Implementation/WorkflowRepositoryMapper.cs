using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Workflows
{
    public class WorkflowRepositoryMapper : BaseEntityMapper<WorkflowRepository, WorkflowRepository>, IWorkflowRepositoryMapper
    {
        public override WorkflowRepository Map(WorkflowRepository entity, WorkflowRepository entityTransformed)
        {
            #region [ Base ]

            entityTransformed.ActiveFrom = entity.ActiveFrom;
            entityTransformed.ActiveTo = entity.ActiveTo;
            entityTransformed.Name = entity.Name;
            entityTransformed.Version = entity.Version;
            entityTransformed.Xaml = entity.Xaml;
            entityTransformed.Json = entity.Json;
            entityTransformed.Status = entity.Status;
            entityTransformed.CustomActivities = entity.CustomActivities;
            entityTransformed.DSWEnvironment = entity.DSWEnvironment;

            #endregion

            return entityTransformed;
        }

    }
}
