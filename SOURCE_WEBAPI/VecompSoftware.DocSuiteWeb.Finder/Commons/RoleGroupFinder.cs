using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class RoleGroupFinder
    {
        public static IEnumerable<SecurityUser> GetRoleGroupsAllAuthorizationType(this IRepository<RoleGroup> repository, short roleId)
        {
            return repository.Query(f => f.Role.EntityShortId == roleId, optimization: true)
                .Include(f => f.SecurityGroup)
                .Include(f => f.SecurityGroup.SecurityUsers)
                .Select()
                .SelectMany(f => f.SecurityGroup.SecurityUsers);
        }
    }
}
