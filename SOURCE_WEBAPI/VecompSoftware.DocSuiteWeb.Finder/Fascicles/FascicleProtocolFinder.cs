using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    /// <summary>
    /// Extending IRepository<FascicleProtocol>
    /// </summary>
    public static class FascicleProtocolFinder
    {
        public static IQueryable<FascicleProtocol> GetByReferenceTypeAndProtocol(this IRepository<FascicleProtocol> repository, Guid uniqueIdProtocol, ReferenceType referenceType)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == uniqueIdProtocol && x.ReferenceType == referenceType)
                .SelectAsQueryable();
        }

        public static int CountByReferenceTypeAndProtocol(this IRepository<FascicleProtocol> repository, Guid uniqueIdProtocol, ReferenceType referenceType)
        {
            return repository.Queryable(true)
                .Where(x => x.DocumentUnit.UniqueId == uniqueIdProtocol && x.ReferenceType == referenceType)
                .Count();
        }

        public static IQueryable<FascicleProtocol> GetByFascicleAndProtocol(this IRepository<FascicleProtocol> repository, Protocol protocol, Guid idFascicle)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == protocol.UniqueId && x.Fascicle.UniqueId == idFascicle)
                .Include(i => i.Fascicle)
                .SelectAsQueryable();
        }
        public static IQueryable<FascicleProtocol> GetByFascicle(this IRepository<FascicleProtocol> repository, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.Fascicle.UniqueId == idFascicle, optimization: optimization)
                .Include(f => f.FascicleFolder)
                .Include(f => f.DocumentUnit.Category)
                .Include(f => f.DocumentUnit.Container)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleProtocol> GetByIdFascicleFolder(this IRepository<FascicleProtocol> repository, Protocol protocol, Guid idFascicleFolder)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == protocol.UniqueId && x.FascicleFolder.UniqueId == idFascicleFolder)
                .Include(i => i.FascicleFolder)
                .SelectAsQueryable();
        }

        public static int CountByFascicleAndIdProtocol(this IRepository<FascicleProtocol> repository, Guid uniqueIdProtocol, Guid idFascicle)
        {
            return repository.Queryable(true)
                .Where(x => x.DocumentUnit.UniqueId == uniqueIdProtocol && x.Fascicle.UniqueId == idFascicle)
                .Count();
        }
    }
}
