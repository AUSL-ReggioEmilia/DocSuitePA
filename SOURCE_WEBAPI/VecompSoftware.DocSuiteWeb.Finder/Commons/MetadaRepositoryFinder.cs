using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class MetadaRepositoryFinder
    {
        public static MetadataRepository GetLatestVersion(this IRepository<MetadataRepository> repository, string name)
        {          
            return repository.Queryable(true).Where(x => x.Name == name).OrderBy(x => x.Version).LastOrDefault();
        }

        public static bool MetadataRepositoryExist(this IRepository<MetadataRepository> repository, string name)
        {
            return repository.Queryable(true).Any(x => x.Name == name);
        }
    }
}
