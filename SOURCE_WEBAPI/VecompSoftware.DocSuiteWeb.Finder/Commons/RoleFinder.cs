using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using RoleTypology = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.RoleTypology;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class RoleFinder
    {
        public static IQueryable<Role> GetByRoleId(this IRepository<Role> repository, short roleId, bool optimization = false)
        {
            return repository
                .Query(x => x.EntityShortId == roleId, optimization: optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<Role> GetByUniqueId(this IRepository<Role> repository, Guid roleUniqueId, bool optimization = false)
        {
            return repository.Query(x => x.UniqueId == roleUniqueId, optimization: optimization)
                .SelectAsQueryable();
        }
        public static ICollection<RoleFullTableValuedModel> FindRoles(this IRepository<Role> repository, string username, string domain, string name, Guid? uniqueId, short? parentId,
            string serviceCode, Guid? idTenantAOO, int? environment, bool? loadOnlyRoot, bool? loadOnlyMy, bool? loadAlsoParent, RoleTypology? roleTypology, short? idCategory,
            Guid? idDossierFolder)
        {
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_Name, DBNull.Value);
            QueryParameter uniqueIdParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_UniqueId, DBNull.Value);
            QueryParameter parentParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_ParentId, DBNull.Value);
            QueryParameter serviceCodeParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_ServiceCode, DBNull.Value);
            QueryParameter idTenantAOOParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_IdTenantAOO, DBNull.Value);
            QueryParameter environmentParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_Environment, DBNull.Value);
            QueryParameter loadOnlyRootParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_LoadOnlyRoot, DBNull.Value);
            QueryParameter loadOnlyMyParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_LoadOnlyMy, DBNull.Value);
            QueryParameter loadAlsoParentParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_LoadAlsoParent, DBNull.Value);
            QueryParameter roleTypologyParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_RoleTypology, DBNull.Value);
            QueryParameter idCategoryParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_IdCategory, DBNull.Value);
            QueryParameter idDossierFolderParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_IdDossierFolder, DBNull.Value);

            if (!string.IsNullOrEmpty(name))
            {
                nameParameter.ParameterValue = name;
            }
            if (uniqueId.HasValue)
            {
                uniqueIdParameter.ParameterValue = uniqueId;
            }
            if (parentId.HasValue)
            {
                parentParameter.ParameterValue = parentId.Value;
            }
            if (!string.IsNullOrEmpty(serviceCode))
            {
                serviceCodeParameter.ParameterValue = serviceCode;
            }
            if (idTenantAOO.HasValue)
            {
                idTenantAOOParameter.ParameterValue = idTenantAOO;
            }
            if (environment.HasValue)
            {
                environmentParameter.ParameterValue = environment;
            }
            if (loadOnlyRoot.HasValue)
            {
                loadOnlyRootParameter.ParameterValue = loadOnlyRoot;
            }
            if (loadOnlyMy.HasValue)
            {
                loadOnlyMyParameter.ParameterValue = loadOnlyMy;
            }
            if (loadAlsoParent.HasValue)
            {
                loadAlsoParentParameter.ParameterValue = loadAlsoParent;
            }
            if (roleTypology.HasValue)
            {
                roleTypologyParameter.ParameterValue = roleTypology;
            }
            if (idCategory.HasValue)
            {
                idCategoryParameter.ParameterValue = idCategory;
            }
            if (idDossierFolder.HasValue)
            {
                idDossierFolderParameter.ParameterValue = idDossierFolder;
            }

            return repository.ExecuteModelFunction<RoleFullTableValuedModel>(CommonDefinition.SQL_FX_Role_FindRoles,
                new QueryParameter(CommonDefinition.SQL_Param_Role_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Role_Domain, domain),
                nameParameter, uniqueIdParameter, parentParameter, serviceCodeParameter, idTenantAOOParameter, environmentParameter, loadOnlyRootParameter,
                loadOnlyMyParameter, loadAlsoParentParameter, roleTypologyParameter, idCategoryParameter, idDossierFolderParameter);
        }

        public static int CountFindRoles(this IRepository<Role> repository, string username, string domain, string name, Guid? uniqueId, short? parentId,
            string serviceCode, Guid? idTenantAOO, int? environment, bool? loadOnlyRoot, bool? loadOnlyMy, bool? loadAlsoParent, RoleTypology? roleTypology,
            short? idCategory, Guid? idDossierFolder)
        {
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_Name, DBNull.Value);
            QueryParameter uniqueIdParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_UniqueId, DBNull.Value);
            QueryParameter parentParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_ParentId, DBNull.Value);
            QueryParameter serviceCodeParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_ServiceCode, DBNull.Value);
            QueryParameter idTenantAOOParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_IdTenantAOO, DBNull.Value);
            QueryParameter environmentParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_Environment, DBNull.Value);
            QueryParameter loadOnlyRootParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_LoadOnlyRoot, DBNull.Value);
            QueryParameter loadOnlyMyParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_LoadOnlyMy, DBNull.Value);
            QueryParameter loadAlsoParentParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_LoadAlsoParent, DBNull.Value);
            QueryParameter roleTypologyParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_RoleTypology, DBNull.Value);
            QueryParameter idCategoryParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_IdCategory, DBNull.Value);
            QueryParameter idDossierFolderParameter = new QueryParameter(CommonDefinition.SQL_Param_Role_IdDossierFolder, DBNull.Value);

            if (!string.IsNullOrEmpty(name))
            {
                nameParameter.ParameterValue = name;
            }
            if (uniqueId.HasValue)
            {
                uniqueIdParameter.ParameterValue = uniqueId;
            }
            if (parentId.HasValue)
            {
                parentParameter.ParameterValue = parentId.Value;
            }
            if (!string.IsNullOrEmpty(serviceCode))
            {
                serviceCodeParameter.ParameterValue = serviceCode;
            }
            if (idTenantAOO.HasValue)
            {
                idTenantAOOParameter.ParameterValue = idTenantAOO;
            }
            if (environment.HasValue)
            {
                environmentParameter.ParameterValue = environment;
            }
            if (loadOnlyRoot.HasValue)
            {
                loadOnlyRootParameter.ParameterValue = loadOnlyRoot;
            }
            if (loadOnlyMy.HasValue)
            {
                loadOnlyMyParameter.ParameterValue = loadOnlyMy;
            }
            if (loadAlsoParent.HasValue)
            {
                loadAlsoParentParameter.ParameterValue = loadAlsoParent;
            }
            if (roleTypology.HasValue)
            {
                roleTypologyParameter.ParameterValue = roleTypology;
            }
            if (idCategory.HasValue)
            {
                idCategoryParameter.ParameterValue = idCategory;
            }
            if (idDossierFolder.HasValue)
            {
                idDossierFolderParameter.ParameterValue = idDossierFolder;
            }

            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Role_CountFindRoles,
                    new QueryParameter(CommonDefinition.SQL_Param_Role_UserName, username),
                    new QueryParameter(CommonDefinition.SQL_Param_Role_Domain, domain),
                    nameParameter, uniqueIdParameter, parentParameter, serviceCodeParameter, idTenantAOOParameter, environmentParameter, loadOnlyRootParameter,
                    loadOnlyMyParameter, loadAlsoParentParameter, roleTypologyParameter, idCategoryParameter, idDossierFolderParameter);
        }
        public static bool HasCategoryFascicleRole(this IRepository<Role> repository, string account, short idCategory)
        {
            int lastNumber = repository.Queryable(true).Count(x => x.RoleUsers.Any(y => y.Account == account)
              && x.CategoryFascicleRights.Any(y => y.CategoryFascicle.Category.EntityShortId == idCategory));
            return lastNumber > 0;
        }
    }
}
