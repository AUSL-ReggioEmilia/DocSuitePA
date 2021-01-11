using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    public static class UDSCollaborationFinder
    {
        public static IQueryable<UDSCollaboration> GetUDSCollaborations(this IRepository<UDSCollaboration> repository, bool optimization = true)
        {
            IQueryable<UDSCollaboration> results = repository
                .Query(optimization)
                .SelectAsQueryable();
            return results;
        }

        public static IQueryable<UDSCollaboration> GetByUDSId(this IRepository<UDSCollaboration> repository, Guid udsId, bool optimization = true)
        {
            return repository.Query(x => x.IdUDS == udsId, optimization)
                .Include(i => i.Relation)
                .SelectAsQueryable();
        }
    }
}
