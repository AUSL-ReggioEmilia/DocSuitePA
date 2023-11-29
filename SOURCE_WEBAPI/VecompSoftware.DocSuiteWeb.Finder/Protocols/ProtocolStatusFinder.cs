using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Protocols
{
    public static class ProtocolStatusFinder
    {
        public static IQueryable<ProtocolStatus> GetByProtocolStatus(this IRepository<ProtocolStatus> repository, string code, bool optimization = true)
        {
            return repository.Query(x => x.Status == code, optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<ProtocolStatus> GetByDescription(this IRepository<ProtocolStatus> repository, string description, bool optimization = true)
        {
            return repository.Query(x => x.ProtocolStatusDescription == description, optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<ProtocolStatus> GetByCodeOrDescription(this IRepository<ProtocolStatus> repository, string description, bool optimization = true)
        {
            return repository.Query(x => x.Status == description || x.ProtocolStatusDescription == description, optimization)
                .SelectAsQueryable();
        }

        public static int CountProtocolStatusEntityByProtocolStatus(this IRepository<ProtocolStatus> repository, string code, bool optimization = false)
        {
            IQueryable<ProtocolStatus> partialQuery = repository.Query(x => x.Status == code, optimization).SelectAsQueryable();
            return partialQuery.Count();
        }

        public static int CountProtocolStatusByCodeOrDescription(this IRepository<ProtocolStatus> repository, string description, bool optimization = false)
        {
            IQueryable<ProtocolStatus> partialQuery = repository.Query(x => x.Status == description || x.ProtocolStatusDescription == description, optimization).SelectAsQueryable();
            return partialQuery.Count();
        }
    }
}
