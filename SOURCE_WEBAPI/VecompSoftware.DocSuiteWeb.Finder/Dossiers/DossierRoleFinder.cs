using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Dossiers
{
    public static class DossierRoleFinder
    {
        public static IQueryable<DossierRole> GetAuthorizedDossierRoles(this IRepository<DossierRole> repository, Guid idDossier, bool optimization = true)
        {
            return repository.Query(d => d.Dossier.UniqueId == idDossier && d.Status == DossierRoleStatus.Active,
                optimization: optimization)
                .Include(d => d.Role.TenantAOO)
                .SelectAsQueryable();
        }

    }
}
