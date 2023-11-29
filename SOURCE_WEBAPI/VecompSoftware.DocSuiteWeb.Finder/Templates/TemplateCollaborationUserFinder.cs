using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Templates
{
    public static class TemplateCollaborationUserFinder
    {
        public static IQueryable<TemplateCollaborationUser> GetTemplatesByUser(this IRepository<TemplateCollaborationUser> repository, string account, short idRole)
        {
            return repository.Query(f => f.Account == account && f.Role.EntityShortId == idRole)
                .Include(f => f.TemplateCollaboration)
                .SelectAsQueryable();
        }

        public static TemplateCollaborationUser GetByRoleId(this IRepository<TemplateCollaborationUser> repository, short idRole)
        {
            return repository.Query(f => f.Role.EntityShortId == idRole).SelectAsQueryable().FirstOrDefault();
        }

        public static IQueryable<TemplateCollaborationUser> GetInvalidatingTemplateCollaborationUsers(this IRepository<TemplateCollaborationUser> repository)
        {
            return repository.Query(f => f.IsValid == false).SelectAsQueryable();
        }
    }
}