using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    /// <summary>
    /// Extending IRepository<FascicleProtocol>
    /// </summary>
    public static class FascicleDocumentUnitFinder
    {
        public static IQueryable<FascicleDocumentUnit> GetByReferenceTypeAndDocumentUnit(this IRepository<FascicleDocumentUnit> repository, Guid uniqueIdDocumentUnit, Entity.Fascicles.ReferenceType referenceType)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == uniqueIdDocumentUnit && x.ReferenceType == referenceType)
                .SelectAsQueryable();
        }

        public static int CountByReferenceTypeAndDocumentUnit(this IRepository<FascicleDocumentUnit> repository, Guid uniqueIdDocumentUnit, Entity.Fascicles.ReferenceType referenceType)
        {
            return repository.Queryable(true)
                .Where(x => x.DocumentUnit.UniqueId == uniqueIdDocumentUnit && x.ReferenceType == referenceType)
                .Count();
        }

        public static IQueryable<FascicleDocumentUnit> GetByFascicleAndDocumentUnit(this IRepository<FascicleDocumentUnit> repository, DocumentUnit documentUnit, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == documentUnit.UniqueId && x.Fascicle.UniqueId == idFascicle, optimization: optimization)
                .Include(i => i.Fascicle)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleDocumentUnit> GetByFascicle(this IRepository<FascicleDocumentUnit> repository, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.Fascicle.UniqueId == idFascicle, optimization: optimization)
                .Include(f => f.FascicleFolder)
                .Include(f => f.DocumentUnit.Category)
                .Include(f => f.DocumentUnit.Container)
                .Include(f => f.DocumentUnit.TenantAOO)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleDocumentUnit> GetByIdFascicleFolder(this IRepository<FascicleDocumentUnit> repository, Guid idFascicleFolder, bool optimization = false)
        {
            return repository.Query(x => x.FascicleFolder.UniqueId == idFascicleFolder, optimization: optimization)
                .Include(i => i.FascicleFolder)
                .Include(i => i.DocumentUnit)
                .SelectAsQueryable();
        }

        public static IQueryable<FascicleDocumentUnit> GetByIdFascicleFolder(this IRepository<FascicleDocumentUnit> repository, DocumentUnit documentUnit, Guid idFascicleFolder, bool optimization = false)
        {
            return repository.Query(x => x.DocumentUnit.UniqueId == documentUnit.UniqueId && x.FascicleFolder.UniqueId == idFascicleFolder, optimization: optimization)
                .Include(i => i.FascicleFolder)
                .Include(i => i.DocumentUnit)
                .SelectAsQueryable();
        }

        public static int CountByFascicleAndDocumentUnit(this IRepository<FascicleDocumentUnit> repository, Guid uniqueIdDocumentUnit, Guid idFascicle)
        {
            return repository.Queryable(true)
                .Where(x => x.DocumentUnit.UniqueId == uniqueIdDocumentUnit && x.Fascicle.UniqueId == idFascicle)
                .Count();
        }
        public static bool HasFascicleDocumentUnit(this IRepository<FascicleDocumentUnit> repository, string folderName, Guid idFascicle)
        {
            bool res = repository.Queryable(true).Any(x => x.FascicleFolder.Name == folderName && x.Fascicle.UniqueId == idFascicle);
            return res;
        }

        public static ICollection<FascicleDocumentUnitTableValuedModel> GetFascicleDocumentUnitsPrivate(this IRepository<FascicleDocumentUnit> repository, Fascicle fascicle, Guid? idFascicleFolder, Guid? idTenantAOO)
        {
            QueryParameter fascicleFolderParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdFascicleFolder, DBNull.Value);
            QueryParameter idTenantAooParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdTenantAOO, DBNull.Value);
            if (idFascicleFolder.HasValue)
            {
                fascicleFolderParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdFascicleFolder, idFascicleFolder.Value);
            }
            if (idTenantAOO.HasValue)
            {
                idTenantAooParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdTenantAOO, idTenantAOO.Value);
            }

            return repository.ExecuteModelFunction<FascicleDocumentUnitTableValuedModel>(CommonDefinition.SQL_FX_FascicleDocumentUnit_FX_GetFascicleDocumentUnits,
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FascicleId, fascicle.UniqueId), fascicleFolderParameter, idTenantAooParameter);
        }

        public static short GetLatestSequenceNumber(this IRepository<FascicleDocumentUnit> repository, Guid idFascicle)
        {
            short? maxSequenceNumber = repository.Queryable(true)
                .Where(x => x.Fascicle.UniqueId == idFascicle)
                .Max(s => (short?)s.SequenceNumber);

            if (maxSequenceNumber.HasValue)
            {
                return maxSequenceNumber.Value;
            }
            return 0;
        }
    }
}
