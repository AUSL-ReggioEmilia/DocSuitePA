using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    public static class FascicleRoleFinder
    {
        public static bool ExistFascicleRolesAccounted(this IRepository<FascicleRole> repository, Guid idFascicle, short roleId)
        {
            return repository
                .Queryable(true)
                .Count(f => f.Fascicle.UniqueId == idFascicle && f.AuthorizationRoleType == Entity.Commons.AuthorizationRoleType.Accounted && f.Role.EntityShortId == roleId) > 0;
        }

        public static IQueryable<FascicleRole> GetFascicleRoleAccounted(this IRepository<FascicleRole> repository, Guid idFascicle, short roleId)
        {
            return repository
                .Query(f => f.Fascicle.UniqueId == idFascicle && f.AuthorizationRoleType == Entity.Commons.AuthorizationRoleType.Accounted && f.Role.EntityShortId == roleId)
                .Include(f => f.Fascicle)
                .SelectAsQueryable();
        }
    }
}
