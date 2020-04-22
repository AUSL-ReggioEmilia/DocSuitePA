using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.DocumentArchives
{
    public static class DocumentSeriesConstraintFinder
    {
        public static int CountExistingConstraint(this IRepository<DocumentSeriesConstraint> repository, string name, int idSeries, Guid uniqueId, bool optimization = true)
        {
            return repository.Queryable(optimization: optimization).Count(x => x.Name == name && x.DocumentSeries.EntityId == idSeries && x.UniqueId != uniqueId);
        }
    }
}
