using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
namespace VecompSoftware.DocSuiteWeb.Finder.Workflows
{
    public static class WorkflowAuthorizationFinder
    {
        public static WorkflowAuthorization GetByUser(this IRepository<WorkflowAuthorization> repository, string account, Guid workflowActivityId)
        {
            return repository
                .Query(x => x.WorkflowActivity.UniqueId == workflowActivityId && x.Account == account)
                .SelectAsQueryable()
                .SingleOrDefault();
        }
    }
}
