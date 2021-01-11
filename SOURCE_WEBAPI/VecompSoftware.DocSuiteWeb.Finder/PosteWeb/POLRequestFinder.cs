using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.PosteOnLineRequests
{
    public static class POLRequestFinder
    {

        public static IQueryable<PosteOnLineRequest> GetByUniqueId(this IRepository<PosteOnLineRequest> repository, Guid uniqueId, bool optimization = false)
        {
            return repository.Query(x => x.UniqueId == uniqueId, optimization)
                             .SelectAsQueryable();
        }

        public static IQueryable<PosteOnLineRequest> GetByDocumentUnitId(this IRepository<PosteOnLineRequest> repository, Guid documentUnitId, bool optimization = false)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == documentUnitId, optimization)
                             .SelectAsQueryable();
        }
    }
}
