using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;


namespace VecompSoftware.DocSuiteWeb.Finder.DocumentUnits
{
    public static class DocumentUnitFinder
    {
        public static IQueryable<DocumentUnit> GetByNumbering(this IRepository<DocumentUnit> repository, short year, int number)
        {
            return repository.Query(x => x.Year == year && x.Number == number)
                .Include(i => i.Category)
                .Include(i => i.Container)
                .SelectAsQueryable();
        }
        public static DocumentUnit GetByNumbering(this IRepository<DocumentUnit> repository, short year, int number, int environment, bool optimization = false)
        {
            return repository.Query(x => x.Year == year && x.Number == number && x.Environment == environment, optimization: optimization)
                .SelectAsQueryable().SingleOrDefault();
        }

        public static IQueryable<DocumentUnit> GetByNumbering(this IRepository<DocumentUnit> repository, Guid uniqueId, int environment)
        {
            return repository.Query(x => x.UniqueId == uniqueId && x.Environment == environment)
                .Include(i => i.DocumentUnitChains)
                .SelectAsQueryable();
        }

        public static IQueryable<DocumentUnit> GetById(this IRepository<DocumentUnit> repository, Guid uniqueId)
        {
            return repository.Query(x => x.UniqueId == uniqueId)
                .Include(i => i.DocumentUnitChains)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }

        public static IQueryable<DocumentUnit> GetByIdWithCategory(this IRepository<DocumentUnit> repository, Guid uniqueId)
        {
            return repository.Query(x => x.UniqueId == uniqueId)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }

        public static IQueryable<DocumentUnit> GetCompleteById(this IRepository<DocumentUnit> repository, Guid uniqueId)
        {
            return repository.Query(x => x.UniqueId == uniqueId)
                .Include(t => t.Category.CategoryFascicles.Select(f => f.FasciclePeriod))
                .Include(i => i.Container)
                .Include(i => i.UDSRepository)
                .Include(i => i.DocumentUnitRoles)
                .Include(i => i.DocumentUnitChains)
                .SelectAsQueryable();
        }

        public static ICollection<DocumentUnitTableValuedModel> GetFascicleDocumentUnitsPrivate(this IRepository<DocumentUnit> repository, Fascicle fascicle, Guid? idFascicleFolder, Guid? idTenantAOO)
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

            return repository.ExecuteModelFunction<DocumentUnitTableValuedModel>(CommonDefinition.SQL_FX_DocumentUnit_FascicleDocumentUnitsPrivate,
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FascicleId, fascicle.UniqueId), fascicleFolderParameter, idTenantAooParameter);
        }

        public static ICollection<DocumentUnitTableValuedModel> GetAuthorized(this IRepository<DocumentUnit> repository, Guid idFascicle, string username, string domain, Guid idTenantAOO)
        {
            return repository.ExecuteModelFunction<DocumentUnitTableValuedModel>(CommonDefinition.SQL_FX_DocumentUnits_AuthorizedDocumentUnitsByFascicle,
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FascicleId, idFascicle), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_UserName, username),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Domain, domain), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Year, DBNull.Value),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Number, DBNull.Value), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_DocumentUnitName, DBNull.Value),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_CategoryId, DBNull.Value), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Subject, DBNull.Value),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdTenantAOO, idTenantAOO));
        }

        public static ICollection<DocumentUnitTableValuedModel> GetAuthorized(this IRepository<DocumentUnit> repository, int skip, int top, Guid idFascicle, string username, string domain, int? year, string number, string documentUnitName, int? categoryId, int? containerId, string subject, bool includeChildClassification, Guid idTenantAOO)
        {
            QueryParameter yearParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Year, DBNull.Value);
            QueryParameter numberParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Number, DBNull.Value);
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_DocumentUnitName, DBNull.Value);
            QueryParameter categoryParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_CategoryId, DBNull.Value);
            QueryParameter containerParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_ContainerId, DBNull.Value);
            QueryParameter subjectParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Subject, DBNull.Value);

            if (year.HasValue)
            {
                yearParameter.ParameterValue = year.Value;
            }

            if (!string.IsNullOrEmpty(number))
            {
                numberParameter.ParameterValue = number;
            }

            if (!string.IsNullOrEmpty(documentUnitName))
            {
                nameParameter.ParameterValue = documentUnitName;
            }

            if (categoryId.HasValue)
            {
                categoryParameter.ParameterValue = categoryId.Value;
            }

            if (containerId.HasValue)
            {
                containerParameter.ParameterValue = containerId.Value;
            }

            if (!string.IsNullOrEmpty(subject))
            {
                subjectParameter.ParameterValue = subject;
            }

            return repository.ExecuteModelFunction<DocumentUnitTableValuedModel>(CommonDefinition.SQL_FX_DocumentUnits_AuthorizedDocumentUnitsByFascicle,
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_UserName, username), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Domain, domain),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FascicleId, idFascicle), yearParameter, numberParameter, nameParameter, categoryParameter, containerParameter, subjectParameter,
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IncludeChildClassification, includeChildClassification),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Skip, skip), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Top, top),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdTenantAOO, idTenantAOO));
        }

        public static int CountByUniqueId(this IRepository<DocumentUnit> repository, Guid uniqueId)
        {
            return repository.Queryable().Count(x => x.UniqueId == uniqueId);
        }

        public static int CountAuthorized(this IRepository<DocumentUnit> repository, Guid idFascicle, string username, string domain, int? year, string number, string documentUnitName, int? categoryId, int? containerId, string subject, bool includeChildClassification, Guid idTenantAOO)
        {
            QueryParameter yearParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Year, DBNull.Value);
            QueryParameter numberParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Number, DBNull.Value);
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_DocumentUnitName, DBNull.Value);
            QueryParameter categoryParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_CategoryId, DBNull.Value);
            QueryParameter containerParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_ContainerId, DBNull.Value);
            QueryParameter subjectParameter = new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Subject, DBNull.Value);

            if (year.HasValue)
            {
                yearParameter.ParameterValue = year.Value;
            }

            if (!string.IsNullOrEmpty(number))
            {
                numberParameter.ParameterValue = number;
            }

            if (!string.IsNullOrEmpty(documentUnitName))
            {
                nameParameter.ParameterValue = documentUnitName;
            }

            if (categoryId.HasValue)
            {
                categoryParameter.ParameterValue = categoryId.Value;
            }

            if (!string.IsNullOrEmpty(subject))
            {
                subjectParameter.ParameterValue = subject;
            }

            if (containerId.HasValue)
            {
                containerParameter.ParameterValue = containerId.Value;
            }

            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_DocumentUnits_CountAuthorizedDocumentUnitsByFascicle,
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_UserName, username), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Domain, domain),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FascicleId, idFascicle), yearParameter, numberParameter, nameParameter, categoryParameter, containerParameter, subjectParameter,
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IncludeChildClassification, includeChildClassification),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdTenantAOO, idTenantAOO));
        }

        public static ICollection<DocumentUnitTableValuedModel> GetAllowedDocumentUnits(this IRepository<DocumentUnit> repository, string username, string domain, DateTimeOffset dateFrom, DateTimeOffset dateTo, Guid idTenantAOO)
        {
            ICollection<DocumentUnitTableValuedModel> results = repository.ExecuteModelFunction<DocumentUnitTableValuedModel>(CommonDefinition.SQL_FX_DocumentUnit_AllowedDocumentUnits,
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_UserName, username), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_DateFrom, dateFrom), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_DateTo, dateTo),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdTenantAOO, idTenantAOO));
            return results;
        }

        public static bool HasVisibilityRight(this IRepository<DocumentUnit> repository, string username, string domain, Guid idDocumentUnit)
        {
            bool results = repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_DocumentUnit_HasVisibilityRight,
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_UserName, username), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdDocumentUnit, idDocumentUnit));
            return results;
        }

        public static ICollection<DocumentUnitTableValuedModel> GetFascicleDocumentUnitsPublic(this IRepository<DocumentUnit> repository, Fascicle fascicle, string filter)
        {
            return repository.ExecuteModelFunction<DocumentUnitTableValuedModel>(CommonDefinition.SQL_FX_DocumentUnit_FascicleDocumentUnitsPublic,
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FascicleId, fascicle.UniqueId),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FilterNameTitle, string.IsNullOrEmpty(filter) ? string.Empty : filter));
        }

        public static IEnumerable<DocumentUnitTableValuedModel> GetFascicolableDocumentUnits(this IRepository<DocumentUnit> repository, string username, string domain,
            DateTimeOffset dateFrom, DateTimeOffset dateTo, bool includeThreshold, string threshold, Guid idTenantAOO, bool excludeLinked)
        {
            return repository.ExecuteModelFunction<DocumentUnitTableValuedModel>(CommonDefinition.SQL_FX_DocumentUnits_FascicolableDocumentUnits,
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_UserName, username), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_DateFrom, dateFrom),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_DateTo, dateTo),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IncludeThreshold, includeThreshold),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_ThresholdFrom, DateTimeOffset.ParseExact(threshold, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_ExcludeLinked, excludeLinked),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdTenantAOO, idTenantAOO));
        }

        public static bool CanBeFascicolable(this IRepository<DocumentUnit> repository, Fascicle fascicle, DocumentUnit documentUnit)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_DocumentUnits_CanBeFascicolable,
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FascicleIdCategory, fascicle.Category.EntityShortId),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FascicleEnvironment, fascicle.DSWEnvironment.HasValue ? fascicle.DSWEnvironment.Value : 0),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FascicleType, (short)fascicle.FascicleType),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdDocumentUnit, documentUnit.UniqueId),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Environment, documentUnit.Environment),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_CategoryId, documentUnit.Category.EntityShortId));
        }

        public static IQueryable<DocumentUnit> GetByUniqueIdWithRole(this IRepository<DocumentUnit> repository, Guid uniqueIdProtocol, bool optimization = false)
        {
            return repository.Query(x => x.UniqueId == uniqueIdProtocol, optimization: optimization)
                .Include(t => t.Category.CategoryFascicles.Select(f => f.FasciclePeriod))
                .Include(i => i.DocumentUnitRoles.Select(p => p.UniqueIdRole))
                .Include(i => i.Container.ProtLocation)
                .SelectAsQueryable();
        }

        public static IQueryable<DocumentUnit> GetFullByUniqueId(this IRepository<DocumentUnit> repository, Guid uniqueIdResolution, bool optimization = false)
        {
            return repository.Query(t => t.UniqueId == uniqueIdResolution, optimization: optimization)
                .Include(t => t.Category.CategoryFascicles.Select(f => f.FasciclePeriod))
                .Include(t => t.Container.ReslLocation)
                .SelectAsQueryable();
        }

        public static IQueryable<DocumentUnit> GetByIdWithUDSRepository(this IRepository<DocumentUnit> repository, Guid uniqueId)
        {
            return repository.Query(x => x.UniqueId == uniqueId)
                .Include(i => i.UDSRepository)
                .SelectAsQueryable();
        }

        public static ICollection<DocumentUnitTableValuedModel> GetDocumentUnitsByChains(this IRepository<DocumentUnit> repository, string username, string domain, ICollection<Guid> chains, Guid idTenantAOO)
        {
            DataTable table = new DataTable();
            table.Columns.Add("val", typeof(string));
            foreach (Guid chain in chains)
            {
                table.Rows.Add(chain);
            }

            ICollection<DocumentUnitTableValuedModel> results = repository.ExecuteModelFunction<DocumentUnitTableValuedModel>(CommonDefinition.SQL_FX_DocumentUnit_DocumentUnitsByChain,
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_UserName, username), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdTenantAOO, idTenantAOO), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Chains, table) { ParameterTypeName = "string_list_tbltype" });
            return results;
        }
    }
}
