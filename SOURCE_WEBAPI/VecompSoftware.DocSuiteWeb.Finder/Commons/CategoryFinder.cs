using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class CategoryFinder
    {

        public static string GetHierarcyCode(this IRepository<Category> repository, short idCategory)
        {
            return repository.ExecuteModelScalarFunction<string>(CommonDefinition.SQL_FX_Category_HierarcyCode, new QueryParameter(CommonDefinition.SQL_Param_Category_IdCategory, idCategory));
        }

        public static string GetHierarcyDescription(this IRepository<Category> repository, short idCategory)
        {
            return repository.ExecuteModelScalarFunction<string>(CommonDefinition.SQL_FX_Category_HierarcyDescription, new QueryParameter(CommonDefinition.SQL_Param_Category_IdCategory, idCategory));
        }

        public static IQueryable<Category> GetById(this IRepository<Category> repository, short idCategory)
        {
            return repository.Query(x => x.EntityShortId == idCategory)
                .SelectAsQueryable();
        }

        public static ICollection<CategoryFullTableValuedModel> FindByIdCategory(this IRepository<Category> repository, string username, string domain, short idCategory, FascicleType? fascicleType)
        {
            QueryParameter fascicleTypeParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_FascicleType, DBNull.Value);
            if (fascicleType.HasValue)
            {
                fascicleTypeParameter.ParameterValue = fascicleType;
            }

            return repository.ExecuteModelFunction<CategoryFullTableValuedModel>(CommonDefinition.SQL_FX_Category_FindCategory,
                   new QueryParameter(CommonDefinition.SQL_Param_Category_UserName, username),
                   new QueryParameter(CommonDefinition.SQL_Param_Category_Domain, domain),
                   new QueryParameter(CommonDefinition.SQL_Param_Category_IdCategory, idCategory),
                   fascicleTypeParameter);
        }

        public static ICollection<CategoryFullTableValuedModel> FindCategories(this IRepository<Category> repository, string username, string domain, string name, FascicleType? fascicleType,
            bool? hasFascicleInsertRights, string manager, string secretary, short? role, bool? loadRoot, short? parentId, bool? parentAllDescendants, string fullCode, short? idContainer, bool? fascicleFilterEnabled)
        {
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_Name, DBNull.Value);
            QueryParameter fascicleTypeParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_FascicleType, DBNull.Value);
            QueryParameter hasFascicleInsertRightsParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_HasFascicleInsertRights, DBNull.Value);
            QueryParameter managerParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_Manager, DBNull.Value);
            QueryParameter secretaryParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_Secretary, DBNull.Value);
            QueryParameter roleParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_Role, DBNull.Value);
            QueryParameter rootParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_LoadRoot, DBNull.Value);
            QueryParameter parentParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_ParentId, DBNull.Value);
            QueryParameter parentDescendantsParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_ParentAllDescendants, DBNull.Value);
            QueryParameter fullCodeParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_FullCode, DBNull.Value);
            QueryParameter containerParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_Container, DBNull.Value);
            QueryParameter fascicleFilterEnabledParameter = new QueryParameter(CommonDefinition.SQL_Param_Category_FascicleFilterEnabled, DBNull.Value);

            if (!string.IsNullOrEmpty(name))
            {
                nameParameter.ParameterValue = name;
            }
            if (fascicleType.HasValue)
            {
                fascicleTypeParameter.ParameterValue = fascicleType.Value;
            }
            if (hasFascicleInsertRights.HasValue)
            {
                hasFascicleInsertRightsParameter.ParameterValue = hasFascicleInsertRights.Value;
            }
            if (!string.IsNullOrEmpty(manager))
            {
                managerParameter.ParameterValue = manager;
            }
            if (!string.IsNullOrEmpty(secretary))
            {
                secretaryParameter.ParameterValue = secretary;
            }
            if (role.HasValue)
            {
                roleParameter.ParameterValue = role.Value;
            }
            if (loadRoot.HasValue)
            {
                rootParameter.ParameterValue = loadRoot.Value;
            }
            if (parentId.HasValue)
            {
                parentParameter.ParameterValue = parentId.Value;
            }
            if (parentAllDescendants.HasValue)
            {
                parentDescendantsParameter.ParameterValue = parentAllDescendants.Value;
            }
            if (!string.IsNullOrEmpty(fullCode))
            {
                fullCodeParameter.ParameterValue = fullCode;
            }
            if (idContainer.HasValue)
            {
                containerParameter.ParameterValue = idContainer.Value;
            }
            if (fascicleFilterEnabled.HasValue)
            {
                fascicleFilterEnabledParameter.ParameterValue = fascicleFilterEnabled.Value;
            }

            return repository.ExecuteModelFunction<CategoryFullTableValuedModel>(CommonDefinition.SQL_FX_Category_FindCategories,
                   new QueryParameter(CommonDefinition.SQL_Param_Category_UserName, username),
                   new QueryParameter(CommonDefinition.SQL_Param_Category_Domain, domain),
                   nameParameter, rootParameter, parentParameter, parentDescendantsParameter, fullCodeParameter, fascicleFilterEnabledParameter, fascicleTypeParameter, hasFascicleInsertRightsParameter,
                   managerParameter, secretaryParameter, roleParameter, containerParameter);
        }
    }
}
