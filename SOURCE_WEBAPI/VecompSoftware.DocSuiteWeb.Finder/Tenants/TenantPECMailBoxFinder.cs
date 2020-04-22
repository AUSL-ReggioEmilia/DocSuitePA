using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Tenants
{
    public static class TenantPECMailBoxFinder
    {
        public static ICollection<PECMailBox> GetTenantPECMailBoxes(this IRepository<Tenant> repository, Guid uniqueId)
        {
            ICollection<PECMailBox> results = repository
                .Query(x => x.UniqueId == uniqueId)
                .Include(x => x.PECMailBoxes)
                .SelectAsQueryable()
                .First()
                .PECMailBoxes;
            return results;
        }
    }
}
