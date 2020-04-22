using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    /// <summary>
    /// Extending IRepository<FascicleDocumentSeriesItem>
    /// </summary>
    public static class FascicleDocumentSeriesItemFinder
    {
        public static IQueryable<FascicleDocumentSeriesItem> GetByFascicleAndDocumentSeries(this IRepository<FascicleDocumentSeriesItem> repository, DocumentSeriesItem documentSeriesItem, Guid idFascicle)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == documentSeriesItem.UniqueId && x.Fascicle.UniqueId == idFascicle)
                .Include(i => i.Fascicle)
                .SelectAsQueryable();
        }
        public static IQueryable<FascicleDocumentSeriesItem> GetByIdFascicleFolder(this IRepository<FascicleDocumentSeriesItem> repository, DocumentSeriesItem documentSeriesItem, Guid idFascicleFolder)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == documentSeriesItem.UniqueId && x.FascicleFolder.UniqueId == idFascicleFolder)
                .Include(i => i.FascicleFolder)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleDocumentSeriesItem> GetByFascicle(this IRepository<FascicleDocumentSeriesItem> repository, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.Fascicle.UniqueId == idFascicle, optimization: optimization)
                .Include(f => f.FascicleFolder)
                 .SelectAsQueryable();
        }

    }
}
