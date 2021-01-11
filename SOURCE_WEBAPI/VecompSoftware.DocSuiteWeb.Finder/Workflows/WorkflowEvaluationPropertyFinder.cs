using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Workflows
{
    public static class WorkflowEvaluationPropertyFinder
    {
        public static IQueryable<WorkflowEvaluationProperty> GetByPropertyName(this IRepository<WorkflowEvaluationProperty> repository, string propertyName, Guid idWorkflowRepository, bool optimization = false)
        {
            return repository.Query(f => f.WorkflowRepository.UniqueId == idWorkflowRepository && f.Name == propertyName, optimization).SelectAsQueryable();
        }
    }
}
