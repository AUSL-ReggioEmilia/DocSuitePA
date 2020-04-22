using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Protocols
{
    public static class ProtocolUserFinder
    {
        public static IQueryable<ProtocolUser> GetLastAuthorizedByProtocol(this IRepository<ProtocolUser> repository, Guid idProtocol, string account, bool optimization = true)
        {
            return repository.Query(x => x.Protocol.UniqueId == idProtocol && x.Type == ProtocolUserType.Authorization && x.Account == account, optimization)
                .OrderBy(o => o.OrderByDescending(oo => oo.RegistrationDate))
                .SelectAsQueryable();
        }
    }
}
