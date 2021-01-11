using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class CategorySchemaFinder
    {
        public static CategorySchema FindActiveCategorySchema(this IRepository<CategorySchema> repository, bool optimization = false)
        {
            return repository.Query(x => x.EndDate == null || x.StartDate > DateTimeOffset.UtcNow, optimization)
                             .SelectAsQueryable()
                             .SingleOrDefault();
        }
    }
}
