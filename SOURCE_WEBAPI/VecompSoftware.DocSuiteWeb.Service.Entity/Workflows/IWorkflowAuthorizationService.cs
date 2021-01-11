using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Workflows
{
    public interface IWorkflowAuthorizationService : IEntityBaseService<WorkflowAuthorization>
    {
        ICollection<WorkflowAuthorization> GetAuthorizationsByMappings(IEnumerable<WorkflowRoleMapping> workflowMappings);
    }
}
