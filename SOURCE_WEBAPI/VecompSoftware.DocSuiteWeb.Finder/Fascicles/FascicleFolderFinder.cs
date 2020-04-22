using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    public static class FascicleFolderFinder
    {
        public static ICollection<FascicleFolderTableValuedModel> GetChildrenByParent(this IRepository<FascicleFolder> repository, Guid idFascicleFolder)
        {
            return repository.ExecuteModelFunction<FascicleFolderTableValuedModel>(CommonDefinition.SQL_FX_FascicleFolder_AllChildrenByParent,
                new QueryParameter(CommonDefinition.SQL_Param_FascicleFolder_IdFascicleFolder, idFascicleFolder));
        }

        public static ICollection<FascicleFolderTableValuedModel> GetParent(this IRepository<FascicleFolder> repository, Guid idFascicleFolder)
        {
            return repository.ExecuteModelFunction<FascicleFolderTableValuedModel>(CommonDefinition.SQL_FX_FascicleFolder_GetParent,
                new QueryParameter(CommonDefinition.SQL_Param_FascicleFolder_IdFascicleFolder, idFascicleFolder));
        }


        public static bool NameAlreadyExists(this IRepository<FascicleFolder> repository, string name, Guid? idParent, Guid idFascicle)
        {
            QueryParameter parentParameter = idParent.HasValue ? new QueryParameter(CommonDefinition.SQL_Param_FascicleFolder_IdFascicleFolder, idParent.Value) :
                                                                 new QueryParameter(CommonDefinition.SQL_Param_FascicleFolder_IdFascicleFolder, DBNull.Value);

            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_FascicleFolder_NameAlreadyExists,
               new QueryParameter(CommonDefinition.SQL_Param_FascicleFolder_FolderName, name), parentParameter,
               new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdFascicle, idFascicle));
        }

        public static bool HasParent(this IRepository<FascicleFolder> repository, Guid? idParent, Guid idFascicle)
        {
            bool res = repository.Queryable(true).Any(x => x.UniqueId == idParent && x.Fascicle.UniqueId == idFascicle);

            return res;
        }

        public static bool HasFascicleDocumentUnit(this IRepository<FascicleDocumentUnit> repository, Guid? idFascicleFolder)
        {
            bool res = repository.Queryable(true).Any(x => x.FascicleFolder.UniqueId == idFascicleFolder);
            return res;
        }
        public static bool HasFascicleDocument(this IRepository<FascicleDocument> repository, Guid? idFascicleFolder)
        {
            bool res = repository.Queryable(true).Any(x => x.FascicleFolder.UniqueId == idFascicleFolder);
            return res;
        }


        public static int CountChildren(this IRepository<FascicleFolder> repository, Guid idFascicleFolder)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_FascicleFolder_CountChildren,
                new QueryParameter(CommonDefinition.SQL_Param_FascicleFolder_IdFascicleFolder, idFascicleFolder));
        }
        public static FascicleFolder GetByFolderId(this IRepository<FascicleFolder> repository, Guid idFascicleFolder, bool optimization = true)
        {
            return repository.Query(ff => ff.UniqueId == idFascicleFolder, optimization: optimization)
                .Include(i => i.Category)
                .SelectAsQueryable()
                .SingleOrDefault();
        }

        public static IQueryable<FascicleFolder> GetByCategoryAndFascicle(this IRepository<FascicleFolder> repository, Guid idFascicle, short idCategory, bool optimization = true)
        {
            return repository.Query(ff => ff.Fascicle.UniqueId == idFascicle &&
                                    ((ff.Category.EntityShortId == idCategory && ff.Typology == Entity.Fascicles.FascicleFolderTypology.SubFascicle)
                                      || (ff.Typology == Entity.Fascicles.FascicleFolderTypology.Fascicle)), optimization: optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleFolder> GetByIdFascicle(this IRepository<FascicleFolder> repository, Guid idFascicle)
        {
            return repository.Query(f => f.Fascicle.UniqueId == idFascicle)
                .Include(i => i.Fascicle)
                .SelectAsQueryable();
        }

        public static ICollection<FascicleDocumentUnit> GetByIdFascicleFolder(this IRepository<FascicleFolder> repository, Guid idFascicleFolder, bool optimization = true)
        {
            return repository.Query(ff => ff.UniqueId == idFascicleFolder, optimization: optimization)
                .Include(i => i.FascicleDocumentUnits)
                .Include(i => i.FascicleDocumentUnits.Select(j => j.DocumentUnit))
                .SelectAsQueryable()
                .FirstOrDefault().FascicleDocumentUnits;
        }

        public static ICollection<FascicleDocument> GetDocumentsByIdFascicleFolder(this IRepository<FascicleFolder> repository, Guid idFascicleFolder, bool optimization = true)
        {
            return repository.Query(ff => ff.UniqueId == idFascicleFolder, optimization: optimization)
                .Include(i => i.FascicleDocuments)
                .SelectAsQueryable()
                .FirstOrDefault().FascicleDocuments;
        }

        public static IQueryable<FascicleFolder> GetInternetFolderByFascicle(this IRepository<FascicleFolder> repository, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.Fascicle.UniqueId == idFascicle && x.Status == Entity.Fascicles.FascicleFolderStatus.Internet, optimization)
                .Include(i => i.Fascicle)
                .SelectAsQueryable();
        }
    }
}

