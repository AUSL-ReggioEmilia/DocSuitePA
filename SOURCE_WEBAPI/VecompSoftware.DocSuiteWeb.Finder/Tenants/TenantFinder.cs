using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Tenants
{
    /// <summary>
    /// Extending IRepository<Tenant>
    /// </summary>
    public static class TenantFinder
    {
        public static IQueryable<Tenant> GetByUniqueId(this IRepository<Tenant> repository, Guid uniqueId, bool optimization = false)
        {
            return repository.Query(x => x.UniqueId == uniqueId, optimization)
                .Include(i => i.TenantAOO)
                .SelectAsQueryable();
        }

        public static Tenant GetIncludeTenantAPP(this IRepository<Tenant> repository, Guid uniqueId, bool optimization = false)
        {
            return repository.Query(x => x.UniqueId == uniqueId, optimization)
                 .Include(i => i.TenantAOO)
                .SelectAsQueryable()
                .SingleOrDefault();
        }

        public static IQueryable<Tenant> GetByTenantName(this IRepository<Tenant> repository, string tenantName, bool optimization = false)
        {
            return repository.Query(x => x.TenantName == tenantName, optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<TenantAOO> GetByTenantAOOName(this IRepository<TenantAOO> repository, string tenantAOOName, bool optimization = false)
        {
            return repository.Query(x => x.Name == tenantAOOName, optimization)
                .SelectAsQueryable();
        }

        public static ICollection<TenantTableValuedModel> GetUserTenants(this IRepository<Tenant> repository, string username, string domain)
        {
            return repository.ExecuteModelFunction<TenantTableValuedModel>(
                CommonDefinition.SQL_FX_Tenant_UserTenants,
                new QueryParameter(CommonDefinition.SQL_Param_Tenant_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Tenant_Domain, domain)
            );
        }

        public static int CountTenant(this IRepository<Tenant> repository, string tenantName)
        {
            return repository.Queryable().Count(x => x.TenantName == tenantName);
        }

        public static int CountTenant(this IRepository<Tenant> repository, Guid uniqueId)
        {
            return repository.Queryable().Count(x => x.UniqueId == uniqueId);
        }

        public static int CountTenantAOO(this IRepository<TenantAOO> repository, Guid uniqueId)
        {
            return repository.Queryable().Count(x => x.UniqueId == uniqueId);
        }
    }
}
