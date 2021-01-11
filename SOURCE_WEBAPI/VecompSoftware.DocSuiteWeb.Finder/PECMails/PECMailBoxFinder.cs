using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.PECMails
{
    public static class PECMailBoxFinder
    {
        public static IQueryable<PECMailBox> GetPECMailBoxes(this IRepository<PECMailBox> repository, bool optimization = true)
        {
            IQueryable<PECMailBox> results = repository
                .Query(optimization)
                .SelectAsQueryable();
            return results;
        }

        public static IQueryable<PECMailBox> GetPECMailByEmail(this IRepository<PECMailBox> repository, string sender, bool optimization = true)
        {
            IQueryable<PECMailBox> results = repository
                .Query(f=> f.MailBoxRecipient == sender, optimization)
                .Include(f=> f.Location)
                .SelectAsQueryable();
            return results;
        }
    }
}
