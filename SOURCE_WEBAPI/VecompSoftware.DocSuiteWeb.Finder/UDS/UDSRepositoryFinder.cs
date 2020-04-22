using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    /// <summary>
    /// Extending IRepository<UDSRepository>
    /// </summary>
    public static class UDSRepositoryFinder
    {
        public static IQueryable<UDSRepository> GetMaxEnvironment(this IRepository<UDSRepository> repository)
        {
            IQueryable<UDSRepository> temp = repository.Query()
                .OrderBy(o => o.OrderByDescending(s => s.DSWEnvironment))
                .SelectAsQueryable();
            return temp;
        }
        public static UDSRepository GetByIdDocumentUnit(this IRepository<UDSRepository> repository, Guid idDocumentUnit, bool optimization = false)
        {
            UDSRepository result = repository.Query(t => t.DocumentUnits.Any(f => f.UniqueId == idDocumentUnit && f.Environment >= 100), optimization)
                .SelectAsQueryable()
                .FirstOrDefault();
            return result;
        }

        public static UDSRepository GetByName(this IRepository<UDSRepository> repository, string name, bool optimization = true)
        {
            UDSRepository result = repository.Query(t => t.Name.Equals(name), optimization).SelectAsQueryable().FirstOrDefault();
            return result;
        }

        public static IEnumerable<int> GetEnvironments(this IRepository<UDSRepository> repository)
        {
            return repository.Query(true).Select(x => x.DSWEnvironment);
        }

        public static IEnumerable<UDSRepositoryTableValuedModel> GetViewableRepositoriesByTypology(this IRepository<UDSRepository> repository, Guid? idUDSTypology, bool pecAnnexedEnabled)
        {
            QueryParameter idUDSTypologyParameter = new QueryParameter(CommonDefinition.SQL_Param_UDSRepository_IdUDSTypology, DBNull.Value);

            if (idUDSTypology.HasValue)
            {
                idUDSTypologyParameter.ParameterValue = idUDSTypology.Value;
            }

            ICollection<UDSRepositoryTableValuedModel> results = repository.ExecuteModelFunction<UDSRepositoryTableValuedModel>(CommonDefinition.SQL_FX_UDSRepository_ViewableRepositoriesByTypology,
               idUDSTypologyParameter, new QueryParameter(CommonDefinition.SQL_Param_UDSRepository_PECAnnexedEnabled, pecAnnexedEnabled));
            return results.OrderBy(r => r.Name);
        }

        public static IEnumerable<UDSRepositoryTableValuedModel> GetInsertableRepositoriesByTypology(this IRepository<UDSRepository> repository, string userName, string domain, Guid? idUDSTypology, bool pecAnnexedEnabled)
        {
            QueryParameter idUDSTypologyParameter = new QueryParameter(CommonDefinition.SQL_Param_UDSRepository_IdUDSTypology, DBNull.Value);

            if (idUDSTypology.HasValue)
            {
                idUDSTypologyParameter.ParameterValue = idUDSTypology.Value;
            }

            ICollection<UDSRepositoryTableValuedModel> results = repository.ExecuteModelFunction<UDSRepositoryTableValuedModel>(CommonDefinition.SQL_FX_UDSRepository_InsertableRepositoriesByTypology,
                new QueryParameter(CommonDefinition.SQL_Param_UDSRepository_UserName, userName), new QueryParameter(CommonDefinition.SQL_Param_UDSRepository_Domain, domain),
                idUDSTypologyParameter, new QueryParameter(CommonDefinition.SQL_Param_UDSRepository_PECAnnexedEnabled, pecAnnexedEnabled));
            return results.OrderBy(r => r.Name);
        }

        ///<summary>
        ///La funzione non ritorna il ModuleXML
        ///</summary>
        public static IEnumerable<UDSRepositoryTableValuedModel> GetAvailableCQRSPublishedUDSRepositories(this IRepository<UDSRepository> repository, Guid? idUDSTypology, string name, string alias, short? idContainer)
        {
            QueryParameter idUDSTypologyParameter = new QueryParameter(CommonDefinition.SQL_Param_UDSRepository_IdUDSTypology, DBNull.Value);
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_UDSRepository_Name, DBNull.Value);
            QueryParameter containerParameter = new QueryParameter(CommonDefinition.SQL_Param_UDSRepository_Container, DBNull.Value);
            QueryParameter aliasParameter = new QueryParameter(CommonDefinition.SQL_Param_UDSRepository_Alias, DBNull.Value);

            if (idUDSTypology.HasValue)
            {
                idUDSTypologyParameter.ParameterValue = idUDSTypology.Value;
            }
            if (!string.IsNullOrEmpty(name))
            {
                nameParameter.ParameterValue = name;
            }
            if (idContainer.HasValue)
            {
                containerParameter.ParameterValue = idContainer.Value;
            }
            if (!string.IsNullOrEmpty(alias))
            {
                aliasParameter.ParameterValue = alias;
            }
            ICollection<UDSRepositoryTableValuedModel> results = repository.ExecuteModelFunction<UDSRepositoryTableValuedModel>(CommonDefinition.SQL_FX_UDSRepository_AvailableCQRSPublishedUDSRepositories,
                idUDSTypologyParameter, nameParameter, aliasParameter, containerParameter);
            return results.OrderBy(r => r.Name);
        }

        public static UDSRepository GetPreviousVersionWithUDSTypologies(this IRepository<UDSRepository> repository, UDSRepository entityUDSRepository)
        {
            IQueryable<UDSRepository> temp = repository.Query(x => x.Name.Equals(entityUDSRepository.Name) && x.ExpiredDate != null && x.Version == entityUDSRepository.Version - 1)
                .Include(t => t.UDSTypologies)
                .SelectAsQueryable();
            return temp.FirstOrDefault();
        }

        public static IQueryable<UDSRepository> GetTenantUDSRepositories(this IRepository<UDSRepository> repository, string tenantName, string udsName, bool optimization = false)
        {
            IQueryable<UDSRepository> results = repository.Query(x => x.Container.Tenants
                    .Any(y => y.TenantName == tenantName) &&
                        (string.IsNullOrEmpty(udsName) || (!string.IsNullOrEmpty(udsName) && x.Name.Contains(udsName))), optimization).SelectAsQueryable();
            return results;
        }
    }
}
