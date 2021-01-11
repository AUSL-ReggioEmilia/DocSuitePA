using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Parameters
{
    public static class ParameterFinder
    {
        public static IQueryable<Parameter> GetParameters(this IRepository<Parameter> repository)
        {
            IQueryable<Parameter> results = repository
                .Query()
                .SelectAsQueryable();
            return results;
        }
        public static IQueryable<Parameter> GetByUniqueId(this IRepository<Parameter> repository, Guid uniqueId)
        {
            return repository.Query(x => x.UniqueId == uniqueId)
                .SelectAsQueryable();
        }
    }
}
