using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.JeepServiceHosts
{
    public static class JeepServiceHostFinder
    {
        public static IQueryable<JeepServiceHost> GetJeepServiceHosts(this IRepository<JeepServiceHost> repository, bool optimization = true)
        {
            IQueryable<JeepServiceHost> results = repository
                .Query(optimization)
                .SelectAsQueryable();
            return results;
        }
    }
}
