using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class MetadataValueFinder
    {
        public static IQueryable<MetadataValue> GetByNameAndFascicle(this IRepository<MetadataValue> repository, string name, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.Fascicle.UniqueId == idFascicle && x.Name == name, optimization)
                .SelectAsQueryable();
        }
    }
}
