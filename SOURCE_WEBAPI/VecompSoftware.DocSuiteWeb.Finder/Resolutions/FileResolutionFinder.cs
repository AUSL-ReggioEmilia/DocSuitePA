using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Resolutions
{
    /// <summary>
    /// Extending IRepository<FileResolution>
    /// </summary>
    public static class FileResolutionFinder
    {

        public static IQueryable<FileResolution> GetByResolution(this IRepository<FileResolution> repository, int idResolution)
        {
            IQueryable<FileResolution> results = repository.Query(f => f.EntityId == idResolution)
                .SelectAsQueryable();
            return results;
        }

    }
}
