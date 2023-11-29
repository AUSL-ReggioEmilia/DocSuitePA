using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Dossiers
{
    public static class DossierFolderFinder
    {
        public static ICollection<DossierFolderTableValuedModel> GetParent(this IRepository<DossierFolder> repository, Guid idDossierFolder)
        {
            return repository.ExecuteModelFunction<DossierFolderTableValuedModel>(CommonDefinition.SQL_FX_DossierFolder_GetParent,
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdDossierFolder, idDossierFolder));
        }

        public static ICollection<DossierFolderTableValuedModel> GetRootDossierFolders(this IRepository<DossierFolder> repository, Guid idDossier, short? status)
        {
            return repository.ExecuteModelFunction<DossierFolderTableValuedModel>(CommonDefinition.SQL_FX_DossierFolder_RootChildren,
                new QueryParameter(CommonDefinition.SQL_Param_Dossier_IdDossier, idDossier),
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_Status, status));
        }

        public static ICollection<DossierFolderTableValuedModel> GetChildrenByParent(this IRepository<DossierFolder> repository, Guid idDossierFolder, short? status)
        {
            return repository.ExecuteModelFunction<DossierFolderTableValuedModel>(CommonDefinition.SQL_FX_DossierFolder_AllChildrenByParent,
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdDossierFolder, idDossierFolder),
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_Status, status));
        }

        public static bool NameAlreadyExists(this IRepository<DossierFolder> repository, string name, Guid? idParent, Guid idDossier)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_DossierFolder_NameAlreadyExists,
               new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_FolderName, name),
               new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdDossierFolder, idParent),
               new QueryParameter(CommonDefinition.SQL_Param_Dossier_IdDossier, idDossier));
        }

        public static bool FascicleAlreadyExists(this IRepository<DossierFolder> repository, Guid idFascicle, Guid? idParent, Guid idDossier)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_DossierFolder_FascicleAlreadyExists,
               new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdFascicle, idFascicle),
               new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdDossierParentFolder, idParent),
               new QueryParameter(CommonDefinition.SQL_Param_Dossier_IdDossier, idDossier));
        }

        public static ICollection<DossierFolderTableValuedModel> FindProcessFolders(this IRepository<DossierFolder> repository, string userName, string domain, string name, Guid idProcess, bool loadOnlyActive, bool loadOnlyMy)
        {
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_Process_Name, DBNull.Value);
            if (!string.IsNullOrEmpty(name))
            {
                nameParameter.ParameterValue = name;
            }

            return repository.ExecuteModelFunction<DossierFolderTableValuedModel>(CommonDefinition.SQL_FX_DossierFolder_FindProcessFolders,
                new QueryParameter(CommonDefinition.SQL_Param_Dossier_UserName, userName),
                new QueryParameter(CommonDefinition.SQL_Param_Dossier_Domain, domain),
                nameParameter,
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_ProcessId, idProcess),
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_LoadOnlyActive, loadOnlyActive),
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_LoadOnlyMy, loadOnlyMy));
        }

        public static int IsParentFascicleFolder(this IRepository<DossierFolder> repository, Guid? idParent, bool optimization = true)
        {
            return repository.Queryable(optimization: optimization).Count(x => x.UniqueId == idParent && (x.Status == Entity.Dossiers.DossierFolderStatus.Fascicle || x.Status == Entity.Dossiers.DossierFolderStatus.FascicleClose));
        }

        public static bool HasProcessAssociated(this IRepository<DossierFolder> repository, Guid idDossier, bool optimization = true)
        {
            return repository.Query(optimization: optimization).SelectAsQueryable()
                .Any(x => x.Dossier.UniqueId == idDossier && x.Dossier.Processes.Count > 0);
        }


        public static int CountChildren(this IRepository<DossierFolder> repository, Guid idDossierFolder, bool? loadOnlyFolders = null)
        {
            QueryParameter loadOnlyFoldersParameter = new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_LoadOnlyFolders, DBNull.Value);

            if (loadOnlyFolders.HasValue)
            {
                loadOnlyFoldersParameter.ParameterValue = loadOnlyFolders.Value;
            }

            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_DossierFolder_CountChildren,
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdDossierFolder, idDossierFolder),
                loadOnlyFoldersParameter);
        }

        public static bool HasFascicleInFolder(this IRepository<DossierFolder> repository, Guid idFascicle)
        {
            bool res = repository.Queryable(true).Any(f => f.Fascicle.UniqueId == idFascicle);

            return res;
        }

        public static IQueryable<DossierFolder> GetByIdFascicle(this IRepository<DossierFolder> repository, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(f => f.Fascicle.UniqueId == idFascicle, optimization: optimization)
                .Include(i => i.Fascicle)
                .SelectAsQueryable();
        }

        public static DossierFolder GetIncludeDossier(this IRepository<DossierFolder> repository, Guid uniqueId, bool optimization = false)
        {
            return repository.Query(f => f.UniqueId == uniqueId, optimization: optimization)
                .Include(i => i.Dossier)
                .Include(i => i.Dossier.Processes)
                .SelectAsQueryable()
                .SingleOrDefault();
        }

        public static ICollection<DossierFolderTableValuedModel> GetAllParentsOfFascicle(this IRepository<DossierFolder> repository, Guid idDossier, Guid idFascicle)
        {
            return repository.ExecuteModelFunction<DossierFolderTableValuedModel>(CommonDefinition.SQL_FX_DossierFolder_GetAllParentsOfFascicle,
                new QueryParameter(CommonDefinition.SQL_Param_Dossier_IdDossier, idDossier),
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdFascicle, idFascicle));
        }

        public static bool HasAssociatedFascicles(this IRepository<DossierFolder> repository, Guid idDossier)
        {
            return repository.Queryable().Any(x => x.Dossier.UniqueId == idDossier && x.Fascicle != null);
        }

        public static ICollection<DossierFolderTableValuedModel> GetChildren(this IRepository<DossierFolder> repository, Guid idDossierFolder, int skip, int top, bool? loadOnlyFolders)
        {
            QueryParameter loadOnlyFoldersParameter = new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_LoadOnlyFolders, DBNull.Value);

            if (loadOnlyFolders.HasValue)
            {
                loadOnlyFoldersParameter.ParameterValue = loadOnlyFolders.Value;
            }

            return repository.ExecuteModelFunction<DossierFolderTableValuedModel>(CommonDefinition.SQL_FX_DossierFolder_GetChildren,
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdDossierFolder, idDossierFolder),
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_Skip, skip),
                new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_Top, top),
                loadOnlyFoldersParameter);
        }
    }
}
