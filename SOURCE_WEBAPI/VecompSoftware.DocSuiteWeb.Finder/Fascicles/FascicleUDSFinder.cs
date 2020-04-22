using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    public static class FascicleUDSFinder
    {
        public static IQueryable<FascicleUDS> GetByFascicleAndUDS(this IRepository<FascicleUDS> repository, Guid idUDS, Guid idFascicle)
        {
            return repository.Query(x => x.IdUDS == idUDS && x.Fascicle.UniqueId == idFascicle)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleUDS> GetByFascicle(this IRepository<FascicleUDS> repository, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.Fascicle.UniqueId == idFascicle, optimization: optimization)
                .Include(f => f.FascicleFolder)
                .SelectAsQueryable();
        }


        public static int CountByFascicleAndUDS(this IRepository<FascicleUDS> repository, UDSRepository udsRepository, Guid udsId, Guid idFascicle)
        {
            return repository.Queryable(true)
                .Where(x => x.UDSRepository.UniqueId == udsRepository.UniqueId && x.IdUDS == udsId && x.Fascicle.UniqueId == idFascicle)
                .Count();
        }

        public static IQueryable<FascicleUDS> GetByReferenceTypeAndUDS(this IRepository<FascicleUDS> repository, Guid idUDSRepository, Guid udsId, ReferenceType referenceType)
        {
            return repository.Query(x => x.UDSRepository.UniqueId == idUDSRepository && x.IdUDS == udsId && x.ReferenceType == referenceType)
                .SelectAsQueryable();
        }

        public static int CountByReferenceTypeAndUDS(this IRepository<FascicleUDS> repository, Guid idUDSRepository, Guid udsId, ReferenceType referenceType)
        {
            return repository.Queryable(true)
                .Where(x => x.UDSRepository.UniqueId == idUDSRepository && x.IdUDS == udsId && x.ReferenceType == referenceType)
                .Count();
        }
    }
}
