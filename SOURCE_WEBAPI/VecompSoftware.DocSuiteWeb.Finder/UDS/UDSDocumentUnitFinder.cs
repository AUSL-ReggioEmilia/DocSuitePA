using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    public static class UDSDocumentUnitFinder
    {
        public static IQueryable<UDSDocumentUnit> GetByUDSId(this IRepository<UDSDocumentUnit> repository, Guid udsId, bool optimization = true)
        {
            return repository.Query(x => x.IdUDS == udsId, optimization)
                .Include(i => i.Relation)
                .SelectAsQueryable();
        }

        public static IQueryable<UDSDocumentUnit> GetByUDSIdAndDocumentUnit(this IRepository<UDSDocumentUnit> repository, Guid udsId, Guid documentUnitId, bool optimization = true)
        {
            return repository.Query(x => x.IdUDS == udsId && x.Relation.UniqueId == documentUnitId, optimization)
                .Include(i => i.Relation)
                .SelectAsQueryable();
        }
    }
}
