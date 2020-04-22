using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.MassimariScarto
{
    public static class MassimarioScartoFinder
    {
        public static IQueryable<MassimarioScarto> GetAllChildrenByParent(this IRepository<MassimarioScarto> repository, Guid parentId, bool includeLogicalDelete)
        {
            return repository.ExecuteModelFunction<MassimarioScarto>(CommonDefinition.SQL_FX_MassimariScarto_AllChildrenByParentd, new QueryParameter(CommonDefinition.SQL_Param_MassimariScarto_ParentId, parentId),
                new QueryParameter(CommonDefinition.SQL_Param_MassimariScarto_IncludeLogicalDelete, includeLogicalDelete))
                .OrderBy(o => o.MassimarioScartoLevel)
                .ThenBy(o => o.Code)
                .AsQueryable();
        }

        public static IQueryable<MassimarioScarto> GetRoot(this IRepository<MassimarioScarto> repository, bool optimization = false)
        {
            return repository.Query(x => x.MassimarioScartoLevel == 0, optimization: optimization)
             .SelectAsQueryable();
        }

        public static IQueryable<MassimarioScarto> GetRootChildren(this IRepository<MassimarioScarto> repository, bool includeLogicalDelete)
        {
            return repository.ExecuteModelFunction<MassimarioScarto>(CommonDefinition.SQL_FX_MassimarioScarto_RootChildren, new QueryParameter(CommonDefinition.SQL_Param_MassimariScarto_IncludeLogicalDelete, includeLogicalDelete))
                .OrderBy(o => o.MassimarioScartoLevel)
                .ThenBy(o => o.Code)
                .AsQueryable();
        }

        public static IQueryable<MassimarioScarto> Get(this IRepository<MassimarioScarto> repository, string name, bool includeLogicalDelete)
        {
            return repository.ExecuteModelFunction<MassimarioScarto>(CommonDefinition.SQL_FX_MassimarioScarto_FilteredMassimari, new QueryParameter(CommonDefinition.SQL_Param_MassimariScarto_Name, name), new QueryParameter(CommonDefinition.SQL_Param_MassimariScarto_FullCode, DBNull.Value),
                new QueryParameter(CommonDefinition.SQL_Param_MassimariScarto_IncludeLogicalDelete, includeLogicalDelete))
                .OrderBy(o => o.MassimarioScartoLevel)
                .ThenBy(o => o.Code)
                .AsQueryable();
        }

        public static IQueryable<MassimarioScarto> Get(this IRepository<MassimarioScarto> repository, string name, string fullCode, bool includeLogicalDelete)
        {
            return repository.ExecuteModelFunction<MassimarioScarto>(CommonDefinition.SQL_FX_MassimarioScarto_FilteredMassimari, new QueryParameter(CommonDefinition.SQL_Param_MassimariScarto_Name, name),
                new QueryParameter(CommonDefinition.SQL_Param_MassimariScarto_FullCode, fullCode), new QueryParameter(CommonDefinition.SQL_Param_MassimariScarto_IncludeLogicalDelete, includeLogicalDelete))
                .OrderBy(o => o.MassimarioScartoLevel)
                .ThenBy(o => o.Code)
                .AsQueryable();
        }

        public static IQueryable<MassimarioScarto> GetParentByPath(this IRepository<MassimarioScarto> repository, string parentPath)
        {
            return repository.Query(x => x.MassimarioScartoPath == parentPath)
                .SelectAsQueryable();
        }

        public static IQueryable<MassimarioScarto> GetByCode(this IRepository<MassimarioScarto> repository, string fullCode)
        {
            return repository.Query(x => x.FullCode == fullCode)
                .SelectAsQueryable();
        }
    }
}
