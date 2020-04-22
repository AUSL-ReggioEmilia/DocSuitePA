using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class PrivacyLevelFinder
    {
        public static int CountByLevel(this IRepository<PrivacyLevel> repository, int level, Guid id, bool optimization = true)
        {
            return repository.Queryable(optimization: optimization).Count(x => x.Level == level && x.UniqueId != id);
        }

        public static int CountByDescription(this IRepository<PrivacyLevel> repository, string description, Guid id, bool optimization = true)
        {
            return repository.Queryable(optimization: optimization).Count(x => x.Description == description && x.UniqueId != id);
        }

        public static int CountByColour(this IRepository<PrivacyLevel> repository, string colour, Guid id, bool optimization = true)
        {
            return repository.Queryable(optimization: optimization).Count(x => x.Colour == colour && x.UniqueId != id);
        }

    }
}
