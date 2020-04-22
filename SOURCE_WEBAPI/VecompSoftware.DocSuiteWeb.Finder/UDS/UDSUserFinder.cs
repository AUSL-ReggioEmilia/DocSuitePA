using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    public static class UDSUserFinder
    {
        public static IQueryable<UDSUser> GetByUDSId(this IRepository<UDSUser> repository, Guid udsId, bool optimization = true)
        {
            return repository.Query(x => x.IdUDS == udsId, optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<UDSUser> IsUserAuthorized(this IRepository<UDSUser> repository, Guid udsId, string domain, string username, bool optimization = true)
        {
            return repository.Query(x => x.IdUDS == udsId && x.Account.Equals(string.Concat(domain, "\\", username)), optimization)
                .SelectAsQueryable();

        }
    }
}
