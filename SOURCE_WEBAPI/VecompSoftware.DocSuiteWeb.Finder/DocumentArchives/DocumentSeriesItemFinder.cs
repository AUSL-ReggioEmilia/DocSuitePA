using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.DocumentArchives
{
    /// <summary>
    /// Extending IRepository<FascicleDocumentSeriesItem>
    /// </summary>
    public static class DocumentSeriesItemFinder
    {
        public static IQueryable<DocumentSeriesItem> GetByUniqueId(this IRepository<DocumentSeriesItem> repository, Guid uniqueId, bool optimization = false)
        {
            return repository.Query(t => t.UniqueId == uniqueId, optimization: optimization)
                .SelectAsQueryable();
        }
        public static IQueryable<DocumentSeriesItem> GetFullByUniqueId(this IRepository<DocumentSeriesItem> repository, Guid uniqueId, bool optimization = false)
        {
            return repository.Query(t => t.UniqueId == uniqueId, optimization: optimization)
                .Include(t => t.Category.CategoryFascicles.Select(f => f.FasciclePeriod))
                .Include(t => t.DocumentSeries.Container.DocumentSeriesLocation)
                .Include(t => t.DocumentSeries.Container.DocumentSeriesAnnexedLocation)
                .Include(t => t.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation)
                .SelectAsQueryable();
        }
    }
}
