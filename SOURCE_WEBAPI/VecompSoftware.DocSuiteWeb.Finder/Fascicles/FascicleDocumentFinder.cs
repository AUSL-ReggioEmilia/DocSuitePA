using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    public static class FascicleDocumentFinder
    {
        public static IQueryable<FascicleDocument> GetByIdFascicleFolder(this IRepository<FascicleDocument> repository, Guid idFascicleFolder, bool optimization = false)
        {
            return repository.Query(x => x.FascicleFolder.UniqueId == idFascicleFolder, optimization: optimization)
                .Include(i => i.FascicleFolder)
                .SelectAsQueryable();
        }
    }
}