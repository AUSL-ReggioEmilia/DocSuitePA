using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    /// <summary>
    /// Extending IRepository<UDSTypology>
    /// </summary>
    public static class UDSTypologyFinder
    {
        public static bool NameAlreadyExists(this IRepository<UDSTypology> repository, string name, Guid uniqueId, bool optimization = true)
        {
            int result = repository.Queryable(optimization: optimization).Count(r => r.Name.Equals(name) && r.UniqueId != uniqueId);
            return result > 0;
        }
    }
}
