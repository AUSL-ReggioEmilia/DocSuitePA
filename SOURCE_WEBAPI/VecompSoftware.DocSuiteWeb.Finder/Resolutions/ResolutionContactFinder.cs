using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Resolutions
{
    /// <summary>
    /// Extending IRepository<ResolutionContact>
    /// </summary>
    public static class ResolutionContactFinder
    {

        public static IQueryable<ResolutionContact> GetByResolution(this IRepository<ResolutionContact> repository, int idResolution, string comunicationType)
        {
            IQueryable<ResolutionContact> results = repository.Query(r => r.IdResolution == idResolution && r.ComunicationType.Equals(comunicationType))
                .Include(i => i.Contact)
                .SelectAsQueryable();
            return results;
        }

    }
}
