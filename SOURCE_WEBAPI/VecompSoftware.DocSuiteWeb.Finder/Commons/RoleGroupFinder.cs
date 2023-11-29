using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class RoleGroupFinder
    {
        public static IEnumerable<SecurityUser> GetRoleGroupsAllAuthorizationType(this IRepository<RoleGroup> repository, Guid roleUniqueId)
        {
            return repository.Query(f => f.Role.UniqueId == roleUniqueId, optimization: true)
                .Include(f => f.SecurityGroup.SecurityUsers)
                .Select()
                .SelectMany(f => f.SecurityGroup.SecurityUsers);
        }

        public static IEnumerable<SecurityUser> GetPECAuthorizedRoleSecurityUsers(this IRepository<RoleGroup> repository, Guid roleUniqueId)
        {
            return repository.Query(f => f.Role.UniqueId == roleUniqueId && f.ProtocolRights.Substring(2, 1) == "1", optimization: true)
                .Include(f => f.SecurityGroup.SecurityUsers)
                .Select()
                .SelectMany(f => f.SecurityGroup.SecurityUsers);
        }

        public static IEnumerable<SecurityUser> GetProtocolAuthorizedRoleSecurityUsers(this IRepository<RoleGroup> repository, Guid roleUniqueId)
        {
            return repository.Query(f => f.Role.UniqueId == roleUniqueId && f.ProtocolRights.StartsWith("1"), optimization: true)
                .Include(f => f.SecurityGroup.SecurityUsers)
                .Select()
                .SelectMany(f => f.SecurityGroup.SecurityUsers);
        }
    }
}
