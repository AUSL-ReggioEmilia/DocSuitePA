using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Tenants
{
    public static class TenantContainerFinder
    {
        public static ICollection<Container> GetTenantContainers(this IRepository<Tenant> repository, Guid uniqueId)
        {
            ICollection<Container> results = repository
                .Query(x => x.UniqueId == uniqueId)
                .Include(x => x.Containers)
                .SelectAsQueryable()
                .First()
                .Containers;
            return results;
        }
    }
}
