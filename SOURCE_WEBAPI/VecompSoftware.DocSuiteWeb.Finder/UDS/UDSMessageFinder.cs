using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    public static class UDSMessageFinder
    {
        public static IQueryable<UDSMessage> GetByUDSId(this IRepository<UDSMessage> repository, Guid udsId, bool optimization = true)
        {
            return repository.Query(x => x.IdUDS == udsId, optimization)
                .Include(i => i.Relation)
                .SelectAsQueryable();
        }
    }
}
