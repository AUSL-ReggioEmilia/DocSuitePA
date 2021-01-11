using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    public static class FascicleLogFinder
    {
        public static IQueryable<FascicleLog> GetLogsByHashValue(this IRepository<FascicleLog> repository, string hashValue)
        {
            return repository.Query(f => f.Hash == hashValue)
                .Include(i => i.Entity)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleLog> GetLogsByIdFascicle(this IRepository<FascicleLog> repository, Guid idFascicle)
        {
            return repository.Query(f => f.Entity.UniqueId == idFascicle)
                .Include(i => i.Entity)
                .SelectAsQueryable();
        }

    }
}
