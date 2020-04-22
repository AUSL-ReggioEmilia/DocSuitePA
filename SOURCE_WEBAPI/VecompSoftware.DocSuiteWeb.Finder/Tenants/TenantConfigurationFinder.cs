using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Tenants
{
    public static class TenantConfigurationFinder
    {
        public static ICollection<TenantConfiguration> GetTenantConfigurations(this IRepository<Tenant> repository, Guid uniqueId)
        {
            ICollection<TenantConfiguration> results = repository
                .Query(x => x.UniqueId == uniqueId)
                .Include(x => x.Configurations)
                .SelectAsQueryable()
                .First()
                .Configurations;
            return results;
        }
    }
}
