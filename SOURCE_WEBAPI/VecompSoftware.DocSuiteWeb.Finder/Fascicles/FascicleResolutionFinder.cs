using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    /// <summary>
    /// Extending IRepository<FascicleResolution>
    /// </summary>
    public static class FascicleResolutionFinder
    {
        public static IQueryable<FascicleResolution> GetByReferenceTypeAndResolution(this IRepository<FascicleResolution> repository, Resolution resolution, ReferenceType referenceType)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == resolution.UniqueId && x.ReferenceType == referenceType)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleResolution> GetByFascicleAndResolution(this IRepository<FascicleResolution> repository, Resolution resolution, Guid idFascicle)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == resolution.UniqueId && x.Fascicle.UniqueId == idFascicle)
                .Include(i => i.Fascicle)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleResolution> GetByIdFascicleFolder(this IRepository<FascicleResolution> repository, Resolution resolution, Guid idFascicleFolder)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == resolution.UniqueId && x.FascicleFolder.UniqueId == idFascicleFolder)
                .Include(i => i.FascicleFolder)
                .SelectAsQueryable();
        }

        public static int CountByFascicleAndResolution(this IRepository<FascicleResolution> repository, Resolution resolution, Guid idFascicle)
        {
            return repository.Queryable(true)
                .Where(x => x.DocumentUnit.UniqueId == resolution.UniqueId && x.Fascicle.UniqueId == idFascicle)
                .Count();
        }

        public static IQueryable<FascicleResolution> GetByFascicle(this IRepository<FascicleResolution> repository, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.Fascicle.UniqueId == idFascicle, optimization: optimization)
                .Include(f => f.FascicleFolder)
                .Include(f => f.DocumentUnit)
                .Include(f => f.DocumentUnit.Container)
                 .Include(f => f.DocumentUnit.Category)
                .SelectAsQueryable();
        }

    }
}
