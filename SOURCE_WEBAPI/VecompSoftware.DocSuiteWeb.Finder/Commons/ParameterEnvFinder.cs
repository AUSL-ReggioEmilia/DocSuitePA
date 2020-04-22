using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class ParameterEnvFinder
    {
        public static IQueryable<ParameterEnv> GetParameter(this IRepository<ParameterEnv> repository, string name)
        {
            return repository.Query(p => p.Name == name, optimization: true).SelectAsQueryable();
        }
    }
}
