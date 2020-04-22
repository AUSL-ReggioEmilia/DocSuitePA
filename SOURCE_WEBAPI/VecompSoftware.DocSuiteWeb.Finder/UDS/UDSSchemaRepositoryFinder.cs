using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    /// <summary>
    /// Extending IRepository<UDSSchemaRepository>
    /// </summary>
    public static class UDSSchemaRepositoryFinder
    {
        public static IQueryable<UDSSchemaRepository> GetCurrentSchema(this IRepository<UDSSchemaRepository> repository, bool optimization = true)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            IQueryable<UDSSchemaRepository> result = repository.Query(x => now >= x.ActiveDate && (x.ExpiredDate == null || now < x.ExpiredDate), optimization: optimization)
                                                               .SelectAsQueryable();
            return result;
        }
    }
}
