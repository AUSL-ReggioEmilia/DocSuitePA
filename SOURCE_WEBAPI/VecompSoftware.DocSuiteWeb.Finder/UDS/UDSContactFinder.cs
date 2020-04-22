using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    public static class UDSContactFinder
    {
        public static IQueryable<UDSContact> GetByUDSId(this IRepository<UDSContact> repository, Guid udsId, bool optimization = true)
        {
            return repository.Query(x => x.IdUDS == udsId, optimization)
                .Include(i => i.Relation)
                .SelectAsQueryable();
        }
    }
}
