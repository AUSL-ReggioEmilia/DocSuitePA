using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Tenants
{
    public static class TenantWorkflowRepositoryFinder
    {
        public static ICollection<WorkflowRepository> GetTenantWorkflowRepositories(this IRepository<Tenant> repository, Guid uniqueId)
        {
            ICollection<WorkflowRepository> results = repository
                .Query(x => x.UniqueId == uniqueId)
                .Include(x => x.TenantWorkflowRepositories)
                .SelectAsQueryable()
                .First()
                .TenantWorkflowRepositories.Select(x => x.WorkflowRepository).ToList();
            return results;
        }
    }
}
