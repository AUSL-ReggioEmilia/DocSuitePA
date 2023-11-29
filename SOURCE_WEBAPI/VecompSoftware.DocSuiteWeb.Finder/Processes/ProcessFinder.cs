using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Model.Entities.Processes;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Processes
{
    public static class ProcessFinder
    {
        public static bool NameAlreadyExists(this IRepository<Process> repository, string name, Guid idProcess, Guid idCategory, bool optimization = true)
        {
            return repository.Queryable(optimization).Count(x => x.Name == name && x.UniqueId != idProcess && x.Category.UniqueId == idCategory) > 0;
        }

        public static ICollection<ProcessTableValuedModel> FindProcesses(this IRepository<Process> repository, string username, string domain, string name, short? categoryId, Guid? dossierId, bool loadOnlyMy, bool isProcessActive)
        {
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_Process_Name, DBNull.Value);
            if (!string.IsNullOrEmpty(name))
            {
                nameParameter.ParameterValue = name;
            }

            QueryParameter dossierIdParameter = new QueryParameter(CommonDefinition.SQL_Param_Process_DossierId, DBNull.Value);
            if (dossierId.HasValue)
            {
                dossierIdParameter.ParameterValue = dossierId;
            }

            QueryParameter categoryIdParameter = new QueryParameter(CommonDefinition.SQL_Param_Process_CategoryId, DBNull.Value);
            if (categoryId.HasValue)
            {
                categoryIdParameter.ParameterValue = categoryId;
            }

            return repository.ExecuteModelFunction<ProcessTableValuedModel>(
                CommonDefinition.SQL_FX_Process_FindProcesses,
                new QueryParameter(CommonDefinition.SQL_Param_Process_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Process_Domain, domain),
                nameParameter,
                dossierIdParameter,
                categoryIdParameter,
                new QueryParameter(CommonDefinition.SQL_Param_Process_LoadOnlyMy, loadOnlyMy),
                new QueryParameter(CommonDefinition.SQL_Param_Process_IsProcessActive, isProcessActive)
            );
        }

        public static ICollection<ProcessTableValuedModel> FindCategoryProcesses(this IRepository<Process> repository, string username, string domain, short categoryId, bool loadOnlyMy, int skip, int top)
        {
            return repository.ExecuteModelFunction<ProcessTableValuedModel>(
                CommonDefinition.SQL_FX_Process_FindCategoryProcesses,
                new QueryParameter(CommonDefinition.SQL_Param_Process_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Process_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Process_CategoryId, categoryId),
                new QueryParameter(CommonDefinition.SQL_Param_Process_LoadOnlyMy, loadOnlyMy),
                new QueryParameter(CommonDefinition.SQL_Param_Process_Skip, skip),
                new QueryParameter(CommonDefinition.SQL_Param_Process_Top, top)
            );
        }

        public static int CountCategoryProcesses(this IRepository<Process> repository, string username, string domain, short categoryId, bool loadOnlyMy)
        {
            return repository.ExecuteModelScalarFunction<int>(
                CommonDefinition.SQL_FX_Process_CountCategoryProcesses,
                new QueryParameter(CommonDefinition.SQL_Param_Process_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Process_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Process_CategoryId, categoryId),
                new QueryParameter(CommonDefinition.SQL_Param_Process_LoadOnlyMy, loadOnlyMy)
            );
        }
    }
}
