using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.PECMails
{
    public static class PECMailBoxConfigurationFinder
    {
        public static IQueryable<PECMailBoxConfiguration> GetPECMailBoxConfigurations(this IRepository<PECMailBoxConfiguration> repository, bool optimization = true)
        {
            IQueryable<PECMailBoxConfiguration> results = repository
                .Query(optimization)
                .SelectAsQueryable();
            return results;
        }
    }
}
