using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Resolutions
{
    public static class ResolutionKindFinder
    {
        public static bool NameAlreadyExists(this IRepository<ResolutionKind> repository, string name, Guid idResolutionKind, bool optimization = true)
        {
            return repository.Queryable(optimization).Count(x => x.Name == name && x.UniqueId != idResolutionKind) > 0;
        }
    }
}
