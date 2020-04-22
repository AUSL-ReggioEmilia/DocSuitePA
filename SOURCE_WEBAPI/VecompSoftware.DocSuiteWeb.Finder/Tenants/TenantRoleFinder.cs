using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Tenants
{
    public static class TenantRoleFinder
    {
        public static ICollection<Role> GetTenantRoles(this IRepository<Tenant> repository, Guid uniqueId)
        {
            ICollection<Role> results = repository
                .Query(x => x.UniqueId == uniqueId)
                .Include(x => x.Roles)
                .SelectAsQueryable()
                .First()
                .Roles;
            return results;
        }
    }
}
