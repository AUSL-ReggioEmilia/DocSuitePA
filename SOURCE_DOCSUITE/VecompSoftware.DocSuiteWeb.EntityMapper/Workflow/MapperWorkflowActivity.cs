using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.DTO.Workflows;
using NHibernate.Transform;
using NHibernate.Criterion;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Workflow
{
    public class MapperWorkflowActivity : BaseEntityMapper<WorkflowActivity, WorkflowActivityResult>
    {
        public MapperWorkflowActivity() : base()
        {

        }
        protected override WorkflowActivityResult TransformDTO(WorkflowActivity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare WorkflowResult se l'entità non è inizializzata");
            }
            WorkflowActivityResult result = new WorkflowActivityResult();
            result.WorkflowActivityId = entity.Id;
            result.WorkflowActivityName = entity.Name;
            result.WorkflowActivityPublicationDate = entity.RegistrationDate;
            result.WorkflowActivityLastChangedDate = entity.LastChangedDate;
            result.WorkflowActivityStatus = entity.Status;
            result.WorkflowActivityType = entity.ActivityType;
            result.WorkflowActivityRequestorUser = entity.RegistrationUser;
            result.WorkflowRepositoryName = entity.WorkflowInstance.WorkflowRepository.Name;
            result.WorkflowSubject = entity.Subject;
            result.WorkflowInstanceId = entity.WorkflowInstance.Id;
            return result;
        }

        protected override IQueryOver<WorkflowActivity, WorkflowActivity> MappingProjection(IQueryOver<WorkflowActivity, WorkflowActivity> queryOver)
        {
            WorkflowActivityResult workflowResult = null;
            WorkflowRepository workflowRepository = null;
            WorkflowInstance workflowInstance = null;
            
            queryOver
                .SelectList(list => list
                    // Mappatura degli oggetti Desk
                    .Select(x => x.Id).WithAlias(() => workflowResult.WorkflowActivityId)
                    .Select(x => x.Name).WithAlias(() => workflowResult.WorkflowActivityName)
                    .Select(x => x.Status).WithAlias(() => workflowResult.WorkflowActivityStatus)
                    .Select(x => x.ActivityType).WithAlias(() => workflowResult.WorkflowActivityType)                   
                    .Select(x => x.RegistrationDate).WithAlias(() => workflowResult.WorkflowActivityPublicationDate)
                    .Select(x => x.RegistrationUser).WithAlias(() => workflowResult.WorkflowActivityRequestorUser)
                    .Select(x=>x.LastChangedDate).WithAlias(()=>workflowResult.WorkflowActivityLastChangedDate)
                    .Select(x => x.Subject).WithAlias(() => workflowResult.WorkflowSubject)
                    // Mappatura degli oggetti WorkflowRepository
                    .Select(() => workflowRepository.Name).WithAlias(() => workflowResult.WorkflowRepositoryName)
                    .Select(() => workflowInstance.Id).WithAlias(() => workflowResult.WorkflowInstanceId)
                    );
            
            return queryOver;
        }
    }
}
