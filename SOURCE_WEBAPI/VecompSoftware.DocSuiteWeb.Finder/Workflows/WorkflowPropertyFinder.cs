using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Workflows
{
    public static class WorkflowPropertyFinder
    {
        public static WorkflowProperty GetByInstanceIdAndPropertyName(this IRepository<WorkflowProperty> repository, Guid workflowInstanceId, string propertyName, bool optimization = true)
        {
            return repository.Query(x => x.WorkflowActivity.WorkflowInstance.UniqueId == workflowInstanceId && x.Name == propertyName, optimization).SelectAsQueryable().FirstOrDefault();
        }
    }
}