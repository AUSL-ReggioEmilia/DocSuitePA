using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Model.Entities.Templates;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Templates
{
    public static class TemplateCollaborationFinder
    {
        public static int CountInvalidTemplateUsers(this IRepository<TemplateCollaboration> repository, Guid idTemplateCollaboration)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Template_CountInvalidTemplateUsers,
                new QueryParameter(CommonDefinition.SQL_Param_Template_IdTemplateCollaboration, idTemplateCollaboration));
        }

        public static int CountInvalidTemplateRoles(this IRepository<TemplateCollaboration> repository, Guid idTemplateCollaboration)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Template_CountInvalidTemplateRoles,
                new QueryParameter(CommonDefinition.SQL_Param_Template_IdTemplateCollaboration, idTemplateCollaboration));
        }

        public static ICollection<TemplateCollaborationModel> GetAuthorized(this IRepository<TemplateCollaboration> repository, string username, string domain)
        {
            ICollection<TemplateCollaborationModel> results = repository.ExecuteModelFunction<TemplateCollaborationModel>(CommonDefinition.SQL_FX_TemplateCollaboration_AuthorizedTemplates,
                new QueryParameter(CommonDefinition.SQL_Param_Template_UserName, username), new QueryParameter(CommonDefinition.SQL_Param_Template_Domain, domain));
            return results;
        }

        public static ICollection<TemplateCollaborationModel> GetInvalidatingTemplatesByRoleUserAccount(this IRepository<TemplateCollaboration> repository, string account, int idRole)
        {
            ICollection<TemplateCollaborationModel> results = repository.ExecuteModelFunction<TemplateCollaborationModel>(CommonDefinition.SQL_FX_TemplateCollaboration_InvalidateTemplatesByUserAccounts,
                new QueryParameter(CommonDefinition.SQL_Param_Template_UserName, account), new QueryParameter(CommonDefinition.SQL_Param_Template_idRole, idRole));
            return results;
        }

        public static int CountNameAlreadyExist(this IRepository<TemplateCollaboration> repository, string name, Guid uniqueId)
        {
            return repository.Query(x => x.Name == name && x.UniqueId != uniqueId, true)
                .SelectAsQueryable()
                .Count();
        }

        public static TemplateCollaboration GetWithRelations(this IRepository<TemplateCollaboration> repository, Guid uniqueId, bool optimization = false)
        {
            return repository.Query(x => x.UniqueId == uniqueId, optimization: optimization)
                .Include(f => f.TemplateCollaborationUsers.Select(r => r.Role))
                .SelectAsQueryable()
                .SingleOrDefault();
        }

        public static TemplateCollaboration GetByName(this IRepository<TemplateCollaboration> repository, string name, bool optimization = false)
        {
            return repository.Query(x => x.Name == name, optimization: optimization)
                .SelectAsQueryable()
                .SingleOrDefault();
        }
    }
}
