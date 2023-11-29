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

        /// <summary>
        /// The sql function will automatically exclude entries of type <see cref="TemplateCollaborationRepresentationType.Folder"/>
        /// </summary>
        public static ICollection<TemplateCollaborationModel> GetAuthorized(this IRepository<TemplateCollaboration> repository, string username, string domain)
        {
            ICollection<TemplateCollaborationModel> results = repository.ExecuteModelFunction<TemplateCollaborationModel>(CommonDefinition.SQL_FX_TemplateCollaboration_AuthorizedTemplates,
                new QueryParameter(CommonDefinition.SQL_Param_Template_UserName, username), new QueryParameter(CommonDefinition.SQL_Param_Template_Domain, domain));
            return results;
        }

        public static ICollection<TemplateCollaborationModel> GetAllParentsOfTemplate(this IRepository<TemplateCollaboration> repository, Guid templateId)
        {
            ICollection<TemplateCollaborationModel> results = repository.ExecuteModelFunction<TemplateCollaborationModel>(CommonDefinition.SQL_FX_TemplateCollaboration_FX_GetAllParentsOfTemplate,
                new QueryParameter(CommonDefinition.SQL_Param_Template_IdTemplateCollaboration, templateId));
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

        /// <summary>
        /// Returns a template collaboration that can be of type <see cref="TemplateCollaborationRepresentationType.FixedTemplates"/>
        /// or <see cref="TemplateCollaborationRepresentationType.Template"/> inclusing the extended properties TemplateCollaborationUsers and their roles
        /// </summary>
        public static TemplateCollaboration GetWithRelations(this IRepository<TemplateCollaboration> repository, Guid uniqueId, bool optimization = false)
        {
            // it makes no sense to include folders in the returned values so we are excluding them
            return repository.Query(x => x.UniqueId == uniqueId
                                 && x.RepresentationType !=  Entity.Templates.TemplateCollaborationRepresentationType.Folder, optimization: optimization)
                .Include(f => f.TemplateCollaborationUsers.Select(r => r.Role.TenantAOO))
                .SelectAsQueryable()
                .SingleOrDefault();
        }

        public static TemplateCollaboration GetByName(this IRepository<TemplateCollaboration> repository, string name, bool optimization = false)
        {
            return repository.Query(x => x.Name == name, optimization: optimization)
                .SelectAsQueryable()
                .SingleOrDefault();
        }

        public static ICollection<TemplateCollaborationModel> GetChildren(this IRepository<TemplateCollaboration> repository, string username, string domain, Guid idParent, short? status)
        {
            ICollection<TemplateCollaborationModel> results = repository.ExecuteModelFunction<TemplateCollaborationModel>(CommonDefinition.SQL_FX_TemplateCollaboration_GetChildren,
                new QueryParameter(CommonDefinition.SQL_Param_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Template_IdParent, idParent),
                status.HasValue ? new QueryParameter(CommonDefinition.SQL_Param_Template_Status, status.Value) : new QueryParameter(CommonDefinition.SQL_Param_Template_Status, DBNull.Value));
            return results;
        }
    }
}
