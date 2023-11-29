using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Resolutions
{
    /// <summary>
    /// Extending IRepository<ResolutionRole>
    /// </summary>
    public static class ResolutionRoleFinder
    {

        public static IQueryable<ResolutionRole> GetByResolution(this IRepository<ResolutionRole> repository, int idResolution)
        {
            IQueryable<ResolutionRole> results = repository.Query(t => t.EntityId == idResolution)
                .Include(i => i.Role.TenantAOO)
                .SelectAsQueryable();
            return results;
        }

    }
}
