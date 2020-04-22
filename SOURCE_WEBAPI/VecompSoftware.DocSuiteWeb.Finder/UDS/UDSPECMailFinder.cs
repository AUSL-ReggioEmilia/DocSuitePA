using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    public static class UDSPECMailFinder
    {
        public static IQueryable<UDSPECMail> GetUDSPECMails(this IRepository<UDSPECMail> repository, bool optimization = true)
        {
            IQueryable<UDSPECMail> results = repository
                .Query(optimization)
                .SelectAsQueryable();
            return results;
        }

        public static IQueryable<UDSPECMail> GetByUDSId(this IRepository<UDSPECMail> repository, Guid udsId, bool optimization = true)
        {
            return repository.Query(x => x.IdUDS == udsId, optimization)
                .Include(i => i.Relation)
                .SelectAsQueryable();
        }
    }
}
