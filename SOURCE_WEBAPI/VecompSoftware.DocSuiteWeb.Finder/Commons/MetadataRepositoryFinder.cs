using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class MetadataRepositoryFinder
    {
        public static int FindExistingRepository(this IRepository<MetadataRepository> repository, string name, bool optimization = true)
        {
            return repository.Queryable(optimization).Count(x => x.Name == name);
        }
    }
}
