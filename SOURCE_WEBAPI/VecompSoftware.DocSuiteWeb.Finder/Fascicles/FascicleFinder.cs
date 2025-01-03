﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Entity.Commons.DSWEnvironmentType;

namespace VecompSoftware.DocSuiteWeb.Finder.Fascicles
{
    /// <summary>
    /// Extending IRepository<Fascicle>
    /// </summary>
    public static class FascicleFinder
    {
        public static Fascicle GetByUniqueId(this IRepository<Fascicle> repository, Guid uniqueId, bool optimization = true)
        {
            return repository.Query(f => f.UniqueId == uniqueId, optimization: optimization)
                .Include(i => i.Category)
                .SelectAsQueryable()
                .SingleOrDefault();
        }

        public static Fascicle GetByTitle(this IRepository<Fascicle> repository, string title, bool optimization = true)
        {
            return repository.Query(f => f.Title == title, optimization: optimization)
                .Include(i => i.Category)
                .SelectAsQueryable()
                .FirstOrDefault();
        }

        public static Fascicle GetDocuments(this IRepository<Fascicle> repository, Guid uniqueId, ChainType type)
        {
            return repository.Query(f => f.UniqueId == uniqueId && f.FascicleDocuments.Any(d => d.ChainType == type), optimization: true)
                .Include(i => i.FascicleDocuments)
                .SelectAsQueryable()
                .SingleOrDefault();
        }

        public static IQueryable<Fascicle> GetByCategory(this IRepository<Fascicle> repository, Category category)
        {
            return repository.Query(x => x.Category.EntityShortId == category.EntityShortId)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }

        public static ICollection<FascicleTableValuedModel> GetByRight(this IRepository<Fascicle> repository, string username, string domain, short idCategory, string name, bool? hasProcess)
        {
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Name, DBNull.Value);
            QueryParameter hasProcessParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_HasProcess, DBNull.Value);

            if (!string.IsNullOrEmpty(name))
            {
                nameParameter.ParameterValue = name;
            }
            if (hasProcess.HasValue)
            {
                hasProcessParameter.ParameterValue = hasProcess;
            }
            return repository.ExecuteModelFunction<FascicleTableValuedModel>(CommonDefinition.SQL_FX_Fascicle_AvailableFascicles,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Category_IdCategory, idCategory),
                nameParameter,
                hasProcessParameter);
        }

        public static IQueryable<Fascicle> GetActiveFascicles(this IRepository<Fascicle> repository, short idCategory)
        {
            return repository.Query(x => x.Category.EntityShortId == idCategory && !x.EndDate.HasValue)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }
        public static IQueryable<Fascicle> GetWithRoles(this IRepository<Fascicle> repository, Guid idFascicle)
        {
            return repository.Query(x => x.UniqueId == idFascicle)
                .Include(i => i.FascicleRoles.Select(f => f.Role))
                .Include(i => i.DossierFolders.Select(f => f.Dossier))
                .SelectAsQueryable();
        }

        //Questo metodo non esegue un controllo sui diritti dell'utente sui fascicoli, perché potrebbe essere stato delegato
        //ad associare il fascicolo
        public static IQueryable<Fascicle> GetNotLinked(this IRepository<Fascicle> repository, Guid idFascicle)
        {
            return repository.Query(x => x.UniqueId != idFascicle
                                     && x.FascicleType != Entity.Fascicles.FascicleType.Activity
                                     && (!x.FascicleLinks.Any() || !x.FascicleLinks.Any(f => f.FascicleLinked.UniqueId == idFascicle)))
            .Include(i => i.Category)
            .Include(i => i.FascicleLinks.Select(t => t.FascicleLinked))
            .SelectAsQueryable();

        }

        //Questo metodo non esegue un controllo sui diritti dell'utente sui fascicoli, perché potrebbe essere stato delegato
        //ad associare il fascicolo
        public static IQueryable<Fascicle> GetNotLinked(this IRepository<Fascicle> repository, Guid idFascicle, int idCategory)
        {
            short _idCategory = Convert.ToInt16(idCategory);
            return repository.Query(x => x.UniqueId != idFascicle && x.Category.EntityShortId == _idCategory
                                     && x.FascicleType != Entity.Fascicles.FascicleType.Activity
                                     && (!x.FascicleLinks.Any() || !x.FascicleLinks.Any(f => f.FascicleLinked.UniqueId == idFascicle)))
            .Include(i => i.Category)
            .Include(i => i.FascicleLinks.Select(t => t.FascicleLinked))
            .SelectAsQueryable();
        }

        public static IQueryable<Fascicle> GetActives(this IRepository<Fascicle> repository, Guid uniqueIdProtocol)
        {
            return repository.Query(x => x.FascicleDocumentUnits.Any(fp => fp.DocumentUnit.UniqueId == uniqueIdProtocol) && !x.EndDate.HasValue)
                .Include(i => i.FascicleDocumentUnits)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }

        public static IQueryable<Fascicle> GetByNumbering(this IRepository<Fascicle> repository, short year, int number, short idCategory, Entity.Fascicles.FascicleType fascicleType)
        {
            return repository.Query(x => x.Year == year && x.Number == number && x.Category.EntityShortId == idCategory && x.FascicleType == fascicleType)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }

        public static int CountByNumbering(this IRepository<Fascicle> repository, short year, int number, short idCategory)
        {
            return repository.Queryable().Count(x => x.Year == year && x.Number == number && x.Category.EntityShortId == idCategory);
        }

        public static IQueryable<Fascicle> GetActivityFascicleByNumbering(this IRepository<Fascicle> repository, short year, int number)
        {
            return repository.Query(x => x.Year == year && x.Number == number && x.FascicleType == Entity.Fascicles.FascicleType.Activity)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }

        public static int CountActivityFascicleByNumbering(this IRepository<Fascicle> repository, short year, int number)
        {
            return repository.Queryable().Count(x => x.Year == year && x.Number == number && x.FascicleType == Entity.Fascicles.FascicleType.Activity);
        }

        public static int GetLatestNumber(this IRepository<Fascicle> repository, short year, int idCategory)
        {
            int? lastNumber = repository.Queryable(true)
                .Where(x => x.Year == year && x.Category.EntityShortId == idCategory)
                .Max(s => (int?)s.Number);

            if (lastNumber.HasValue)
            {
                return lastNumber.Value;
            }
            return 0;
        }

        public static ICollection<FascicleTableValuedModel> GetAvailables(this IRepository<Fascicle> repository, string username, string domain, Guid documentUnitId)
        {
            return repository.ExecuteModelFunction<FascicleTableValuedModel>(CommonDefinition.SQL_FX_Fascicle_AvailableFasciclesFromDocumentUnit,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UniqueIdDocumentUnit, documentUnitId));
        }

        public static IQueryable<Fascicle> GetAssociated(this IRepository<Fascicle> repository, Resolution resolution, bool optimization = true)
        {
            return repository.Query(x => x.FascicleDocumentUnits.Any(fr => fr.DocumentUnit.UniqueId == resolution.UniqueId && fr.DocumentUnit.Environment == (int)DSWEnvironmentType.Resolution), optimization)
                .Include(i => i.Category)
                .Include(i => i.FascicleDocumentUnits.Select(s => s.DocumentUnit))
                .Include(i => i.FascicleLinks.Select(f => f.FascicleLinked))
                .SelectAsQueryable();
        }

        public static IQueryable<Fascicle> GetAssociated(this IRepository<Fascicle> repository, Resolution resolution, Entity.Fascicles.ReferenceType referenceType, bool optimization = true)
        {
            return repository.Query(x => x.FascicleDocumentUnits.Any(fr => fr.DocumentUnit.UniqueId == resolution.UniqueId && fr.ReferenceType == referenceType && fr.DocumentUnit.Environment == (int)DSWEnvironmentType.Resolution), optimization)
                .Include(i => i.FascicleLinks.Select(f => f.FascicleLinked))
                .SelectAsQueryable();
        }

        public static IQueryable<Fascicle> GetAssociated(this IRepository<Fascicle> repository, Protocol protocol, Entity.Fascicles.ReferenceType referenceType, bool optimization = true)
        {
            return repository.Query(x => x.FascicleDocumentUnits.Any(fp => fp.DocumentUnit.UniqueId == protocol.UniqueId && fp.ReferenceType == referenceType && fp.DocumentUnit.Environment == (int)DSWEnvironmentType.Protocol), optimization)
                .Include(i => i.FascicleLinks.Select(f => f.FascicleLinked))
                .SelectAsQueryable();
        }

        public static IQueryable<Fascicle> GetAssociated(this IRepository<Fascicle> repository, DocumentUnit documentUnit, bool optimization = true)
        {
            return repository.Query(x => x.FascicleDocumentUnits.Any(fu => fu.DocumentUnit.UniqueId == documentUnit.UniqueId), optimization)
                .Include(i => i.Category)
                .Include(i => i.FascicleDocumentUnits.Select(f => f.DocumentUnit))
                .Include(i => i.FascicleLinks.Select(f => f.FascicleLinked))
                .SelectAsQueryable();
        }

        public static IQueryable<Fascicle> GetAssociated(this IRepository<Fascicle> repository, DocumentUnit documentUnit, Entity.Fascicles.ReferenceType referenceType, bool optimization = true)
        {
            return repository.Query(x => x.FascicleDocumentUnits.Any(fu => fu.UniqueId == documentUnit.UniqueId && fu.ReferenceType == referenceType), optimization)
                .Include(i => i.FascicleLinks.Select(f => f.FascicleLinked))
                .SelectAsQueryable();
        }

        public static ICollection<FascicleTableValuedModel> GetPeriodicFromDocumentUnit(this IRepository<Fascicle> repository, DocumentUnit documentUnit)
        {
            return repository.ExecuteModelFunction<FascicleTableValuedModel>(CommonDefinition.SQL_FX_Fascicle_PeriodicFasciclesFromDocumentUnit,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UniqueIdDocumentUnit, documentUnit.UniqueId), new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Environment, documentUnit.Environment));
        }

        public static ICollection<ContactTableValuedModel> GetContacts(this IRepository<Fascicle> repository, Fascicle fascicle)
        {
            return repository.ExecuteModelFunction<ContactTableValuedModel>(CommonDefinition.SQL_FX_Fascicle_GetFascicleContactModels,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdFascicle, fascicle.UniqueId));
        }

        public static bool GetIfCurrentUserIsManagerOnActivityFascicle(this IRepository<Fascicle> repository, string accountName, Guid fascicleId)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Fascicle_IsCurrentUserManagerOnActivityFascicle,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Description, accountName), new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_FascicleId, fascicleId));
        }

        public static bool HasDocumentVisibilityRights(this IRepository<Fascicle> repository, string username, string domain, Guid idFascicle)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Fascicle_HasDocumentVisibilityRight,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdFascicle, idFascicle));
        }

        public static bool AnyActivePeriodicByCategoryAndEnvironment(this IRepository<Fascicle> repository, short idCategory, int environment)
        {
            bool res = repository.Queryable(true).Any(x => x.Category.EntityShortId == idCategory
                                                                                        && x.FascicleType == Entity.Fascicles.FascicleType.Period
                                                                                        && (x.EndDate > DateTime.UtcNow || x.EndDate == null)
                                                                                        && x.StartDate < DateTime.UtcNow
                                                                                        && x.DSWEnvironment == environment);
            return res;
        }

        public static bool ExistActivePeriodic(this IRepository<Fascicle> repository, short idCategory, int environment, short? idContainer)
        {
            bool res = repository.Queryable(true).Any(x => x.Category.EntityShortId == idCategory
                                                                                        && x.FascicleType == Entity.Fascicles.FascicleType.Period
                                                                                        && (x.EndDate > DateTime.UtcNow || x.EndDate == null)
                                                                                        && x.StartDate < DateTime.UtcNow
                                                                                        && x.DSWEnvironment == environment
                                                                                        && (!idContainer.HasValue || x.Container.EntityShortId == idContainer));
            return res;
        }

        public static bool HasFascicolatedDocumentUnits(this IRepository<Fascicle> repository, Guid idFascicle)
        {
            bool res = repository.Queryable(true).Any(x => x.UniqueId == idFascicle && x.FascicleDocumentUnits.Any(y => y.ReferenceType == Entity.Fascicles.ReferenceType.Fascicle));
            return res;
        }

        public static bool HasDocumentUnits(this IRepository<Fascicle> repository, Guid idFascicle)
        {
            bool res = repository.Queryable(true).Any(x => x.UniqueId == idFascicle && x.FascicleDocumentUnits.Any());
            return res;
        }

        public static bool HasInsertRight(this IRepository<Fascicle> repository, string username, string domain, Entity.Fascicles.FascicleType fascicleType)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Fascicle_HasInsertRight,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_FascicleType, (int)fascicleType));
        }

        public static bool HasManageableRight(this IRepository<Fascicle> repository, string username, string domain, Guid idFascicle)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Fascicle_HasManageableRight,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdFascicle, idFascicle));
        }

        public static bool HasViewableRight(this IRepository<Fascicle> repository, string username, string domain, Guid idFascicle)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Fascicle_HasViewableRight,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdFascicle, idFascicle));
        }

        public static ICollection<FascicleTableValuedModel> GetAuthorized(this IRepository<Fascicle> repository, string userName, string domain, int skip, int top, short? year, DateTimeOffset? startDateFrom, DateTimeOffset? startDateTo,
            DateTimeOffset? endDateFrom, DateTimeOffset? endDateTo, int? fascicleStatus, string manager, string name, string subject, bool? viewConfidential, bool? viewAccessible,
            string note, string rack, Guid? idMetadataRepository, string genericMetadataValue, IDictionary<string, string> metadataValues, string classifications, bool? includeChildClassifications,
            IEnumerable<int> roles, int? masterRole, short? container, bool? applySecurity, bool descendingOrder, bool? viewOnlyClosable, DateTimeOffset? thresholdDate, string title, bool isManager, bool isSecretary,
             Guid? idProcess, Guid? idDossierFolder, string processLabel, string dossierFolderLabel, bool optimization = true)
        {
            QueryParameter yearParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Year, DBNull.Value);
            QueryParameter startDateFromParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_StartDateFrom, DBNull.Value);
            QueryParameter startDateToParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_StartDateTo, DBNull.Value);
            QueryParameter endDateFromParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_EndDateFrom, DBNull.Value);
            QueryParameter endDateToParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_EndDateTo, DBNull.Value);
            QueryParameter fascicleStatusParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_FascicleStatus, DBNull.Value);
            QueryParameter managerParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Manager, DBNull.Value);
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Name, DBNull.Value);
            QueryParameter subjectParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Subject, DBNull.Value);
            QueryParameter viewConfidentialParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ViewConfidential, DBNull.Value);
            QueryParameter viewAccessibleParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ViewAccessible, DBNull.Value);
            QueryParameter noteParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Note, DBNull.Value);
            QueryParameter rackParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Rack, DBNull.Value);
            QueryParameter idMetadataRepositoryParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdMetadataRepository, DBNull.Value);
            QueryParameter genericMetadataValueParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_MetadataValue, DBNull.Value);
            QueryParameter classificationsParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Classifications, DBNull.Value);
            QueryParameter includeChildClassificationsParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IncludeChildClassifications, DBNull.Value);
            QueryParameter rolesParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Roles, DBNull.Value);
            QueryParameter masterRoleParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_MasterRole, DBNull.Value);
            QueryParameter containerParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Container, DBNull.Value);
            QueryParameter applySecurityParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ApplySecurity, DBNull.Value);
            QueryParameter metadataValuesParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_MetadataValues, DBNull.Value) { ParameterTypeName = "keyvalue_tbltype" };
            QueryParameter viewOnlyClosableParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ViewOnlyClosable, DBNull.Value);
            QueryParameter thresholdDateParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ThresholdDate, DBNull.Value);
            QueryParameter titleParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Title, DBNull.Value);
            QueryParameter isManagerParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IsManager, DBNull.Value);
            QueryParameter isSecretaryParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IsSecretary, DBNull.Value);
            QueryParameter idProcessParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdProcess, DBNull.Value);
            QueryParameter idDossierFolderParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdDossierFolder, DBNull.Value);
            QueryParameter processLabelParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ProcessLabel, DBNull.Value);
            QueryParameter dossierFolderLabelParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_DossierFolderLabel, DBNull.Value);

            if (year.HasValue)
            {
                yearParameter.ParameterValue = year.Value;
            }
            if (startDateFrom.HasValue)
            {
                startDateFromParameter.ParameterValue = startDateFrom.Value;
            }
            if (startDateTo.HasValue)
            {
                startDateToParameter.ParameterValue = startDateTo.Value;
            }
            if (endDateFrom.HasValue)
            {
                endDateFromParameter.ParameterValue = endDateFrom.Value;
            }
            if (endDateTo.HasValue)
            {
                endDateToParameter.ParameterValue = endDateTo.Value;
            }
            if (fascicleStatus.HasValue)
            {
                fascicleStatusParameter.ParameterValue = fascicleStatus.Value;
            }
            if (!string.IsNullOrEmpty(manager))
            {
                managerParameter.ParameterValue = manager;
            }
            if (!string.IsNullOrEmpty(name))
            {
                nameParameter.ParameterValue = name;
            }
            if (!string.IsNullOrEmpty(subject))
            {
                subjectParameter.ParameterValue = subject;
            }
            if (viewConfidential.HasValue)
            {
                viewConfidentialParameter.ParameterValue = viewConfidential.Value;
            }
            if (viewAccessible.HasValue)
            {
                viewAccessibleParameter.ParameterValue = viewAccessible.Value;
            }
            if (!string.IsNullOrEmpty(note))
            {
                noteParameter.ParameterValue = note;
            }
            if (!string.IsNullOrEmpty(rack))
            {
                rackParameter.ParameterValue = rack;
            }
            if (idMetadataRepository.HasValue)
            {
                idMetadataRepositoryParameter.ParameterValue = idMetadataRepository.Value;
            }
            if (!string.IsNullOrEmpty(genericMetadataValue))
            {
                genericMetadataValueParameter.ParameterValue = genericMetadataValue;
            }
            if (!string.IsNullOrEmpty(classifications))
            {
                classificationsParameter.ParameterValue = classifications;
            }
            if (includeChildClassifications.HasValue)
            {
                includeChildClassificationsParameter.ParameterValue = includeChildClassifications.Value;
            }
            if (applySecurity.HasValue)
            {
                applySecurityParameter.ParameterValue = applySecurity.Value;
            }
            if (viewOnlyClosable.HasValue)
            {
                viewOnlyClosableParameter.ParameterValue = viewOnlyClosable.Value;
            }
            if (thresholdDate.HasValue)
            {
                thresholdDateParameter.ParameterValue = thresholdDate.Value;
            }
            if (roles != null && roles.Any())
            {
                rolesParameter.ParameterValue = string.Join("|", roles);
            }
            if (masterRole.HasValue)
            {
                masterRoleParameter.ParameterValue = masterRole.Value;
            }
            if (container.HasValue)
            {
                containerParameter.ParameterValue = container.Value;
            }
            if (!string.IsNullOrEmpty(title))
            {
                titleParameter.ParameterValue = title;
            }
            isManagerParameter.ParameterValue = isManager;
            isSecretaryParameter.ParameterValue = isSecretary;

            if (idProcess.HasValue)
            {
                idProcessParameter.ParameterValue = idProcess.Value;
            }
            if (idDossierFolder.HasValue)
            {
                idDossierFolderParameter.ParameterValue = idDossierFolder.Value;
            }
            if (!string.IsNullOrEmpty(processLabel))
            {
                processLabelParameter.ParameterValue = processLabel;
            }
            if (!string.IsNullOrEmpty(dossierFolderLabel))
            {
                dossierFolderLabelParameter.ParameterValue = dossierFolderLabel;
            }

            DataTable table = new DataTable();
            table.Columns.Add("KeyName", typeof(string));
            table.Columns.Add("Value", typeof(string));
            if (metadataValues != null && metadataValues.Any())
            {
                foreach (KeyValuePair<string, string> metadataValue in metadataValues)
                {
                    table.Rows.Add(metadataValue.Key, metadataValue.Value);
                }
            }
            metadataValuesParameter.ParameterValue = table;

            ICollection<FascicleTableValuedModel> results = repository.ExecuteModelFunction<FascicleTableValuedModel>(CommonDefinition.SQL_FX_Fascicle_AuthorizedFascicles,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, userName), new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Skip, skip), new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Top, top),
                yearParameter, startDateFromParameter, startDateToParameter, endDateFromParameter, endDateToParameter, fascicleStatusParameter, managerParameter, nameParameter, viewConfidentialParameter, viewAccessibleParameter,
                subjectParameter, noteParameter, rackParameter, idMetadataRepositoryParameter, genericMetadataValueParameter, classificationsParameter, includeChildClassificationsParameter,
                rolesParameter, masterRoleParameter, containerParameter, applySecurityParameter, metadataValuesParameter, viewOnlyClosableParameter, thresholdDateParameter, titleParameter, isManagerParameter, isSecretaryParameter,
                   idProcessParameter, idDossierFolderParameter, processLabelParameter, dossierFolderLabelParameter);

            // L'ordinamento della lista dei fascicoli (StartDate DESC) deve essere determinato solo dalla funzione sql Fascicles_FX_AuthorizedFascicles.
            // Il parametro ForceDescendingOrderElements non deve influenzare tale ordinamento.
            //if (descendingOrder)
            //{
            //    return results.OrderByDescending(x => x.Year).OrderByDescending(x => x.Number).ToList();
            //}

            //return results.OrderBy(x => x.Year).OrderBy(x => x.Number).ToList();

            return results;
        }

        public static int CountAuthorized(this IRepository<Fascicle> repository, string userName, string domain, short? year, DateTimeOffset? startDateFrom, DateTimeOffset? startDateTo,
            DateTimeOffset? endDateFrom, DateTimeOffset? endDateTo, int? fascicleStatus, string manager, string name, string subject, bool? viewConfidential, bool? viewAccessible,
            string note, string rack, Guid? idMetadataRepository, string genericMetadataValue, IDictionary<string, string> metadataValues, string classifications, bool? includeChildClassifications,
            IEnumerable<int> roles, int? masterRole, short? container, bool? applySecurity, bool? viewOnlyClosable, DateTimeOffset? thresholdDate, string title, bool isManager, bool isSecretary,
            Guid? idProcess, Guid? idDossierFolder, string processLabel, string dossierFolderLabel, bool optimization = true)
        {
            QueryParameter yearParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Year, DBNull.Value);
            QueryParameter startDateFromParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_StartDateFrom, DBNull.Value);
            QueryParameter startDateToParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_StartDateTo, DBNull.Value);
            QueryParameter endDateFromParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_EndDateFrom, DBNull.Value);
            QueryParameter endDateToParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_EndDateTo, DBNull.Value);
            QueryParameter fascicleStatusParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_FascicleStatus, DBNull.Value);
            QueryParameter managerParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Manager, DBNull.Value);
            QueryParameter nameParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Name, DBNull.Value);
            QueryParameter subjectParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Subject, DBNull.Value);
            QueryParameter viewConfidentialParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ViewConfidential, DBNull.Value);
            QueryParameter viewAccessibleParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ViewAccessible, DBNull.Value);
            QueryParameter noteParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Note, DBNull.Value);
            QueryParameter rackParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Rack, DBNull.Value);
            QueryParameter idMetadataRepositoryParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdMetadataRepository, DBNull.Value);
            QueryParameter genericMetadataValueParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_MetadataValue, DBNull.Value);
            QueryParameter classificationsParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Classifications, DBNull.Value);
            QueryParameter includeChildClassificationsParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IncludeChildClassifications, DBNull.Value);
            QueryParameter rolesParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Roles, DBNull.Value);
            QueryParameter masterRoleParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_MasterRole, DBNull.Value);
            QueryParameter containerParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Container, DBNull.Value);
            QueryParameter applySecurityParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ApplySecurity, DBNull.Value);
            QueryParameter metadataValuesParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_MetadataValues, DBNull.Value) { ParameterTypeName = "keyvalue_tbltype" };
            QueryParameter viewOnlyClosableParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ViewOnlyClosable, DBNull.Value);
            QueryParameter thresholdDateParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ThresholdDate, DBNull.Value);
            QueryParameter titleParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Title, DBNull.Value);
            QueryParameter isManagerParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IsManager, DBNull.Value);
            QueryParameter isSecretaryParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IsSecretary, DBNull.Value);
            QueryParameter idProcessParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdProcess, DBNull.Value);
            QueryParameter idDossierFolderParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdDossierFolder, DBNull.Value);
            QueryParameter processLabelParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_ProcessLabel, DBNull.Value);
            QueryParameter dossierFolderLabelParameter = new QueryParameter(CommonDefinition.SQL_Param_Fascicle_DossierFolderLabel, DBNull.Value);

            if (year.HasValue)
            {
                yearParameter.ParameterValue = year.Value;
            }
            if (startDateFrom.HasValue)
            {
                startDateFromParameter.ParameterValue = startDateFrom.Value;
            }
            if (startDateTo.HasValue)
            {
                startDateToParameter.ParameterValue = startDateTo.Value;
            }
            if (endDateFrom.HasValue)
            {
                endDateFromParameter.ParameterValue = endDateFrom.Value;
            }
            if (endDateTo.HasValue)
            {
                endDateToParameter.ParameterValue = endDateTo.Value;
            }
            if (fascicleStatus.HasValue)
            {
                fascicleStatusParameter.ParameterValue = fascicleStatus.Value;
            }
            if (!string.IsNullOrEmpty(manager))
            {
                managerParameter.ParameterValue = manager;
            }
            if (!string.IsNullOrEmpty(name))
            {
                nameParameter.ParameterValue = name;
            }
            if (!string.IsNullOrEmpty(subject))
            {
                subjectParameter.ParameterValue = subject;
            }
            if (viewConfidential.HasValue)
            {
                viewConfidentialParameter.ParameterValue = viewConfidential.Value;
            }
            if (viewAccessible.HasValue)
            {
                viewAccessibleParameter.ParameterValue = viewAccessible.Value;
            }
            if (!string.IsNullOrEmpty(note))
            {
                noteParameter.ParameterValue = note;
            }
            if (!string.IsNullOrEmpty(rack))
            {
                rackParameter.ParameterValue = rack;
            }
            if (idMetadataRepository.HasValue)
            {
                idMetadataRepositoryParameter.ParameterValue = idMetadataRepository.Value;
            }
            if (!string.IsNullOrEmpty(genericMetadataValue))
            {
                genericMetadataValueParameter.ParameterValue = genericMetadataValue;
            }
            if (!string.IsNullOrEmpty(classifications))
            {
                classificationsParameter.ParameterValue = classifications;
            }
            if (includeChildClassifications.HasValue)
            {
                includeChildClassificationsParameter.ParameterValue = includeChildClassifications.Value;
            }
            if (applySecurity.HasValue)
            {
                applySecurityParameter.ParameterValue = applySecurity.Value;
            }
            if (viewOnlyClosable.HasValue)
            {
                viewOnlyClosableParameter.ParameterValue = viewOnlyClosable.Value;
            }
            if (thresholdDate.HasValue)
            {
                thresholdDateParameter.ParameterValue = thresholdDate.Value;
            }
            if (roles != null && roles.Any())
            {
                rolesParameter.ParameterValue = string.Join("|", roles);
            }
            if (masterRole.HasValue)
            {
                masterRoleParameter.ParameterValue = masterRole;
            }
            if (container.HasValue)
            {
                containerParameter.ParameterValue = container.Value;
            }
            if (!string.IsNullOrEmpty(title))
            {
                titleParameter.ParameterValue = title;
            }
            isManagerParameter.ParameterValue = isManager;
            isSecretaryParameter.ParameterValue = isSecretary;

            if (idProcess.HasValue)
            {
                idProcessParameter.ParameterValue = idProcess.Value;
            }
            if (idDossierFolder.HasValue)
            {
                idDossierFolderParameter.ParameterValue = idDossierFolder.Value;
            }

            if (!string.IsNullOrEmpty(processLabel))
            {
                processLabelParameter.ParameterValue = processLabel;
            }
            if (!string.IsNullOrEmpty(dossierFolderLabel))
            {
                dossierFolderLabelParameter.ParameterValue = dossierFolderLabel;
            }

            DataTable table = new DataTable();
            table.Columns.Add("KeyName", typeof(string));
            table.Columns.Add("Value", typeof(string));
            if (metadataValues != null && metadataValues.Any())
            {
                foreach (KeyValuePair<string, string> metadataValue in metadataValues)
                {
                    table.Rows.Add(metadataValue.Key, metadataValue.Value);
                }
            }
            metadataValuesParameter.ParameterValue = table;

            int countFascicles = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Fascicle_CountAuthorizedFascicles,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, userName), new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                yearParameter, startDateFromParameter, startDateToParameter, endDateFromParameter, endDateToParameter, fascicleStatusParameter, managerParameter, nameParameter, viewConfidentialParameter, viewAccessibleParameter,
                subjectParameter, noteParameter, rackParameter, idMetadataRepositoryParameter, genericMetadataValueParameter, classificationsParameter, includeChildClassificationsParameter,
                rolesParameter, masterRoleParameter, containerParameter, applySecurityParameter, metadataValuesParameter, viewOnlyClosableParameter, thresholdDateParameter, titleParameter, isManagerParameter, isSecretaryParameter,
                idProcessParameter, idDossierFolderParameter, processLabelParameter, dossierFolderLabelParameter);

            return countFascicles;
        }

        public static bool IsManager(this IRepository<Fascicle> repository, string username, string domain, Guid idFascicle)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Fascicle_IsManager,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdFascicle, idFascicle));
        }

        public static bool HasProcedureDistributionInsertRight(this IRepository<Fascicle> repository, string username, string domain, short idCategory)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Fascicle_HasProcedureDistributionInsertRight,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdCategory, idCategory));
        }

        public static IQueryable<Fascicle> GetAvailableProcedureFasciclesByCategory(this IRepository<Fascicle> repository, short idCategory, bool optimization = true)
        {
            return repository.Query(x => x.FascicleType == Entity.Fascicles.FascicleType.Procedure && x.Category.EntityShortId == idCategory && !x.EndDate.HasValue, optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<Fascicle> GetByMetadataIdentifier(this IRepository<Fascicle> repository, string name, string identifier, bool optimization = true)
        {
            return repository.Query(x => x.SourceMetadataValues.Any(xx => xx.Name == name && xx.ValueString == identifier), optimization)
                .SelectAsQueryable();
        }

        public static int CountAuthorizedFasciclesFromDocumentUnit(this IRepository<Fascicle> repository, string username, string domain, Guid uniqueIdDocumentUnit)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Fascicle_CountAuthorizedFasciclesFromDocumentUnit,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UniqueIdDocumentUnit, uniqueIdDocumentUnit));
        }

        public static ICollection<FascicleTableValuedModel> AuthorizedFasciclesFromDocumentUnit(this IRepository<Fascicle> repository, string username, string domain, Guid uniqueIdDocumentUnit)
        {
            return repository.ExecuteModelFunction<FascicleTableValuedModel>(CommonDefinition.SQL_FX_Fascicle_AuthorizedFasciclesFromDocumentUnit,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UniqueIdDocumentUnit, uniqueIdDocumentUnit));
        }

        public static IQueryable<Fascicle> GetWithDocuments(this IRepository<Fascicle> repository, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.UniqueId == idFascicle, optimization)
                .Include(i => i.FascicleDocuments)
                .SelectAsQueryable();
        }

        public static IQueryable<Fascicle> GetWithContacts(this IRepository<Fascicle> repository, Guid idFascicle, bool optimization = false)
        {
            return repository.Query(x => x.UniqueId == idFascicle, optimization)
                .Include(i => i.Contacts)
                .SelectAsQueryable();
        }

        public static int CountAuthorizedCategoryFascicles(this IRepository<Fascicle> repository, string username, string domain, short idCategory)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Fascicle_CountAuthorizedCategoryFascicles,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Category_IdCategory, idCategory));
        }

        public static ICollection<FascicleTableValuedModel> GetAuthorizedCategoryFascicles(this IRepository<Fascicle> repository, string username, string domain, short idCategory, int skip, int top)
        {
            return repository.ExecuteModelFunction<FascicleTableValuedModel>(CommonDefinition.SQL_FX_Fascicle_FindAuthorizedCategoryFascicles,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Category_IdCategory, idCategory),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Skip, skip),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Top, top));
        }

        public static ICollection<ViewableFascicleTableValuedModel> FasciclesFromDocumentUnit(this IRepository<Fascicle> repository, string username, string domain, Guid idDocumentUnit) {
            return repository.ExecuteModelFunction<ViewableFascicleTableValuedModel>(CommonDefinition.SQL_FX_Fascicle_FX_FasciclesFromDocumentUnit,
                    new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username),
                    new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                    new QueryParameter(CommonDefinition.SQL_Param_DocumentUnit_IdDocumentUnit, idDocumentUnit));
        }


        public static string GetAllDossierFolderLabelName(this IRepository<Fascicle> repository, Guid idDossierFolder) 
        {
            return repository.ExecuteModelScalarFunction<string>(CommonDefinition.SQL_FX_DossierFolder_FX_GetAllDossierFolderLabelName, new QueryParameter(CommonDefinition.SQL_Param_DossierFolder_IdDossierFolder, idDossierFolder));
        }
    }
}
