using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Dossiers
{
    /// <summary>
    /// Extending IRepository<Dossier>
    /// </summary>
    public static class DossierFinder
    {

        public static ICollection<DossierTableValuedModel> GetAuthorized(this IRepository<Dossier> repository, string userName, string domain, int skip, int top, short? year, int? number, string subject, short? idContainer,
            string startDateFrom, string startDateTo, string endDateFrom, string endDateTo, string note, Guid? idMetadataRepository, string metadataValue, bool optimization = true)
        {
            QueryParameter yearParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_Year, DBNull.Value);
            QueryParameter numberParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_Number, DBNull.Value);
            QueryParameter subjectParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_Subject, DBNull.Value);
            QueryParameter containerParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_Container, DBNull.Value);
            QueryParameter startDateFromParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_StartDateFrom, DBNull.Value);
            QueryParameter startDateToParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_StartDateTo, DBNull.Value);
            QueryParameter endDateFromParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_EndDateFrom, DBNull.Value);
            QueryParameter endDateToParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_EndDateTo, DBNull.Value);
            QueryParameter noteParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_Note, DBNull.Value);
            QueryParameter idMetadataRepositoryParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_IdMetadataRepository, DBNull.Value);
            QueryParameter metadataValueParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_MetadataValue, DBNull.Value);

            if (year.HasValue)
            {
                yearParameter.ParameterValue = year.Value;
            }
            if (number.HasValue)
            {
                numberParameter.ParameterValue = number.Value;
            }
            if (!string.IsNullOrEmpty(subject))
            {
                subjectParameter.ParameterValue = subject;
            }
            if (!string.IsNullOrEmpty(note))
            {
                noteParameter.ParameterValue = note;
            }
            if (idContainer.HasValue)
            {
                containerParameter.ParameterValue = idContainer.Value;
            }
            if (!string.IsNullOrEmpty(startDateFrom))
            {
                startDateFromParameter.ParameterValue = startDateFrom;
            }
            if (!string.IsNullOrEmpty(startDateTo))
            {
                startDateToParameter.ParameterValue = startDateTo;
            }
            if (!string.IsNullOrEmpty(endDateFrom))
            {
                endDateFromParameter.ParameterValue = endDateFrom;
            }
            if (!string.IsNullOrEmpty(endDateTo))
            {
                endDateToParameter.ParameterValue = endDateTo;
            }
            if (idMetadataRepository.HasValue)
            {
                idMetadataRepositoryParameter.ParameterValue = idMetadataRepository.Value;
            }
            if (!string.IsNullOrEmpty(metadataValue))
            {
                metadataValueParameter.ParameterValue = metadataValue;
            }

            ICollection<DossierTableValuedModel> results = repository.ExecuteModelFunction<DossierTableValuedModel>(CommonDefinition.SQL_FX_Dossier_AuthorizedDossiers,
                new QueryParameter(CommonDefinition.SQL_Param_Dossier_UserName, userName), new QueryParameter(CommonDefinition.SQL_Param_Dossier_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Dossier_Skip, skip), new QueryParameter(CommonDefinition.SQL_Param_Dossier_Top, top),
                yearParameter, numberParameter, subjectParameter, noteParameter, containerParameter, idMetadataRepositoryParameter, metadataValueParameter, startDateFromParameter, startDateToParameter, endDateFromParameter, endDateToParameter);

            return results.OrderBy(x => x.Year).OrderBy(x => x.Number).ToList();
        }

        public static int CountAuthorized(this IRepository<Dossier> repository, string userName, string domain, short? year, int? number, string subject, short? idContainer, Guid? idMetadataRepository, string metadataValue, bool optimization = true)
        {
            QueryParameter yearParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_Year, DBNull.Value);
            QueryParameter numberParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_Number, DBNull.Value);
            QueryParameter subjectParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_Subject, DBNull.Value);
            QueryParameter containerParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_Container, DBNull.Value);
            QueryParameter matadataRepositoryParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_MetadataRepository, DBNull.Value);
            QueryParameter metadataValueParameter = new QueryParameter(CommonDefinition.SQL_Param_Dossier_MetadataValue, DBNull.Value);
            if (year.HasValue)
            {
                yearParameter.ParameterValue = year.Value;
            }
            if (number.HasValue)
            {
                numberParameter.ParameterValue = number.Value;
            }
            if (!string.IsNullOrEmpty(subject))
            {
                subjectParameter.ParameterValue = subject;
            }
            if (idContainer.HasValue)
            {
                containerParameter.ParameterValue = idContainer.Value;
            }
            if (idMetadataRepository != null)
            {
                matadataRepositoryParameter.ParameterValue = idMetadataRepository;
            }
            if (!string.IsNullOrEmpty(metadataValue))
            {
                metadataValueParameter.ParameterValue = metadataValue;
            }

            int result = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Dossier_CountAuthorizedDossiers,
                new QueryParameter(CommonDefinition.SQL_Param_Dossier_UserName, userName), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_Domain, domain),
                yearParameter, numberParameter, subjectParameter, containerParameter, matadataRepositoryParameter, metadataValueParameter);

            return result;
        }

        public static IQueryable<Dossier> GetCompleteDossier(this IRepository<Dossier> repository, Guid uniqueId, bool optimization = true)
        {
            return repository.Query(d => d.UniqueId == uniqueId, optimization: optimization)
                .Include(i => i.Container)
                .SelectAsQueryable();
        }

        public static IQueryable<Dossier> GetByUniqueId(this IRepository<Dossier> repository, Guid uniqueId, bool loadRelations = false, bool optimization = true)
        {
            if (loadRelations)
            {
                return repository.Query(d => d.UniqueId == uniqueId, optimization: optimization)
                        .Include(x => x.Container)
                        .Include(x => x.DossierRoles)
                        .Include(x => x.DossierRoles.Select(y => y.Role))
                        .Include(x => x.DossierFolders)
                    .SelectAsQueryable();
            }
            return repository.Query(d => d.UniqueId == uniqueId, optimization: optimization).SelectAsQueryable();
        }

        public static IQueryable<Dossier> GetByYearAndNumber(this IRepository<Dossier> repository, short year, int number, bool loadRelations = false, bool optimization = true)
        {
            if (loadRelations)
            {
                return repository.Query(d => d.Year == year && d.Number == number, optimization: optimization)
                        .Include(x => x.Container)
                        .Include(x => x.DossierRoles)
                        .Include(x => x.DossierRoles.Select(y => y.Role))
                        .Include(x => x.DossierFolders)
                    .SelectAsQueryable();
            }
            return repository.Query(d => d.Year == year && d.Number == number, optimization: optimization).SelectAsQueryable();
        }

        public static ICollection<ContactTableValuedModel> GetDossierContacts(this IRepository<Dossier> repository, Guid idDossier)
        {
            return repository.ExecuteModelFunction<ContactTableValuedModel>(CommonDefinition.SQL_FX_Dossier_GetDossierContacts, new QueryParameter(CommonDefinition.SQL_Param_Dossier_IdDossier, idDossier));
        }

        public static int CountDossierByNumbering(this IRepository<Dossier> repository, short year, int number, bool optimization = true)
        {
            return repository.Queryable(optimization: optimization).Count(x => x.Year == year && x.Number == number);
        }

        public static int GetLatestNumber(this IRepository<Dossier> repository, short year, bool optimization = true)
        {
            int? lastNumber = repository.Queryable(optimization: optimization).Where(x => x.Year == year).Max(s => (int?)s.Number);
            if (lastNumber.HasValue)
            {
                return lastNumber.Value;
            }
            return 0;
        }
        
        public static bool HasDossierViewable(this IRepository<Dossier> repository, Guid idDossier, string username, string domain)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Dossier_HasViewableRight,
               new QueryParameter(CommonDefinition.SQL_Param_Dossier_UserName, username),
               new QueryParameter(CommonDefinition.SQL_Param_Dossier_Domain, domain),
               new QueryParameter(CommonDefinition.SQL_Param_Dossier_IdDossier, idDossier));
        }
        
        public static bool HasDossierManageable(this IRepository<Dossier> repository, Guid idDossier, string username, string domain)
        {
            bool results = repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Dossier_HasManageableRight,
              new QueryParameter(CommonDefinition.SQL_Param_Dossier_UserName, username),
              new QueryParameter(CommonDefinition.SQL_Param_Dossier_Domain, domain),
              new QueryParameter(CommonDefinition.SQL_Param_Dossier_IdDossier, idDossier));
            return results;

        }

        public static bool HasDossierInsertRight(this IRepository<Dossier> repository, string username, string domain)
        {
            bool results = repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Dossier_HasInsertRight,
              new QueryParameter(CommonDefinition.SQL_Param_Dossier_UserName, username),
              new QueryParameter(CommonDefinition.SQL_Param_Dossier_Domain, domain));
            return results;
        }

        public static bool HasDossierModifyRight(this IRepository<Dossier> repository, string username, string domain, Guid idDossier)
        {
            bool results = repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Dossier_HasModifyRight,
              new QueryParameter(CommonDefinition.SQL_Param_Dossier_UserName, username),
              new QueryParameter(CommonDefinition.SQL_Param_Dossier_Domain, domain),
              new QueryParameter(CommonDefinition.SQL_Param_Dossier_IdDossier, idDossier));
            return results;
        }

        public static IQueryable<Dossier> GetWithRoles(this IRepository<Dossier> repository, Guid idDossier)
        {
            return repository.Query(x => x.UniqueId == idDossier)
                .Include(i => i.DossierRoles.Select(f => f.Role))
                .SelectAsQueryable();
        }

        public static IQueryable<Dossier> GetWithProcesses(this IRepository<Dossier> repository, Guid idDossier, bool optimization = false)
        {
            return repository.Query(x => x.UniqueId == idDossier, optimization:optimization)
                .Include(i => i.Processes)
                .SelectAsQueryable();
        }

        public static bool HasRootNode(this IRepository<Dossier> repository, Guid idDossier)
        {
            bool results = repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Dossier_HasRootNode,
              new QueryParameter(CommonDefinition.SQL_Param_Dossier_IdDossier, idDossier));
            return results;
        }
    }
}
