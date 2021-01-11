using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    public static class UDSLogFinder
    {
        public static IQueryable<UDSLog> GetMyLogs(this IRepository<UDSLog> repository, Guid idUDS, int skip, int top, string domain, string username, bool optimization = true)
        {
            return repository.Query(x => x.IdUDS == idUDS && x.RegistrationUser.Equals(string.Concat(domain, "\\", username)), optimization)
             .SelectAsQueryable()
             .OrderByDescending(r => r.RegistrationDate).Skip(skip).Take(top);

        }
    }
}
