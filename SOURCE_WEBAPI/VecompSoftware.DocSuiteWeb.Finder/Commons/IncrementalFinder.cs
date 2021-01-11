using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    /// <summary>
    /// Extending IRepository<Collaboration>
    /// </summary>
    public static class IncrementalFinder
    {
        public static IQueryable<Incremental> GetLastCollaborationIncremental(this IRepository<Incremental> repository)
        {
            return repository.Query(t => t.Name.Equals("Collaboration")).SelectAsQueryable();
        }
    }
}
