using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Workflows
{
    public static class WorkflowInstanceFinder
    {
        public static IQueryable<WorkflowInstance> GetByWorkflowName(this IRepository<WorkflowInstance> repository, string name)
        {
            return repository
                .Query(x => x.InstanceId.HasValue && x.WorkflowRepository.Name == name, optimization: true)
                .Include(f => f.WorkflowRepository)
                .SelectAsQueryable();
        }

        public static int CountActiveInstances(this IRepository<WorkflowInstance> repository, Guid workflowRepositoryId, string subject)
        {
            return repository
                .Queryable(optimization: true)
                .Where(x => x.WorkflowRepository.UniqueId == workflowRepositoryId && x.Subject == subject && x.Status <= WorkflowStatus.Progress)
                .Count();
        }
    }
}
