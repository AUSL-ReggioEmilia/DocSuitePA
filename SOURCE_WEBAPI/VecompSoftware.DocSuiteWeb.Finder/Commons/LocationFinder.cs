using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class LocationFinder
    {
        public static bool NameAlreadyExists(this IRepository<Location> repository, string name, Guid idLocation, bool optimization = true)
        {
            return repository.Queryable(optimization).Count(x => x.Name == name && x.UniqueId != idLocation) > 0;
        }
    }
}