using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class UserLogFinder
    {

        public static UserLog GetBySystemUser(this IRepository<UserLog> repository, string systemUser, bool optimization = false)
        {
            return repository.Query(x => x.SystemUser == systemUser, optimization: optimization).SelectAsQueryable().FirstOrDefault();
        }
    }
}
