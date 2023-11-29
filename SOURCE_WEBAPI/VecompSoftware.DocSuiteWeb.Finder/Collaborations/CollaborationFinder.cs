using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Collaborations
{
    /// <summary>
    /// Extending IRepository<Collaboration>
    /// </summary>
    public static class CollaborationFinder
    {
        public static ICollection<CollaborationTableValuedModel> GetProposed(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder));

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_ProposedCollaborations,
                queryParameters.ToArray());
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static int CountProposed(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, usePagination: false));

            int countResults = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Collaboration_CountProposedCollaborations,
                queryParameters.ToArray());
            return countResults;
        }

        public static ICollection<CollaborationTableValuedModel> GetSigning(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, needIsRequired: true));

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_CollaborationsSigning,
                queryParameters.ToArray());
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static int CountSigning(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, needIsRequired: true, usePagination: false));

            int countResults = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Collaboration_CountCollaborationsSigning,
                queryParameters.ToArray());
            return countResults;
        }

        public static ICollection<CollaborationTableValuedModel> GetDelegationSigning(this IRepositoryAsync<Collaboration> repository, ICollection<string> delegations,
            CollaborationFinderModel collaborationFinder)
        {

            DataTable table = new DataTable();
            table.Columns.Add("val", typeof(string));
            foreach (string delegation in delegations)
            {
                table.Rows.Add(delegation);
            }

            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_Signers, table) { ParameterTypeName = "string_list_tbltype" }
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, needIsRequired: true));

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_CollaborationsDelegationSigning,
                queryParameters.ToArray());
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static int CountDelegationSigning(this IRepositoryAsync<Collaboration> repository, ICollection<string> delegations,
            CollaborationFinderModel collaborationFinder)
        {
            DataTable table = new DataTable();
            table.Columns.Add("val", typeof(string));
            foreach (string delegation in delegations)
            {
                table.Rows.Add(delegation);
            }

            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_Signers, table) { ParameterTypeName = "string_list_tbltype" }
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, needIsRequired: true, usePagination: false));

            int countResults = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Collaboration_CountCollaborationsDelegationSigning,
                queryParameters.ToArray());
            return countResults;
        }

        public static ICollection<CollaborationTableValuedModel> GetAllUsers(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder));

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_AllUserCollaborations,
            queryParameters.ToArray());
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static int CountAllUsers(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, usePagination: false));

            int countResults = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Collaboration_CountAllUserCollaborations,
            queryParameters.ToArray());
            return countResults;
        }

        public static ICollection<CollaborationTableValuedModel> GetActiveUsers(this IRepositoryAsync<Collaboration> repository, string userName, ICollection<string> signers,
            CollaborationFinderModel collaborationFinder)
        {
            DataTable table = new DataTable();
            table.Columns.Add("val", typeof(string));
            foreach (string sign in signers)
            {
                table.Rows.Add(sign);
            }

            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName),
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_Signers, table) { ParameterTypeName = "string_list_tbltype" }
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder));

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_ActiveUserCollaborations,
                queryParameters.ToArray());
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static int CountActiveUsers(this IRepositoryAsync<Collaboration> repository, string userName, ICollection<string> signers,
            CollaborationFinderModel collaborationFinder)
        {
            DataTable table = new DataTable();
            table.Columns.Add("val", typeof(string));
            foreach (string sign in signers)
            {
                table.Rows.Add(sign);
            }

            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName),
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_Signers, table) { ParameterTypeName = "string_list_tbltype" }
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, usePagination: false));

            int countResults = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Collaboration_CountActiveUserCollaborations,
                queryParameters.ToArray());
            return countResults;
        }

        public static ICollection<CollaborationTableValuedModel> GetAlreadySigned(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder));

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_AlreadySignedCollaborations,
                queryParameters.ToArray());
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static int CountAlreadySigned(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, usePagination: false));

            int countResults = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Collaboration_CountAlreadySignedCollaborations,
                queryParameters.ToArray());
            return countResults;
        }

        public static ICollection<CollaborationTableValuedModel> GetManagings(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder));

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_CollaborationsManaging,
                queryParameters.ToArray());
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static int CountManagings(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, usePagination: false));

            int countResults = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Collaboration_CountCollaborationsManaging,
                queryParameters.ToArray());
            return countResults;
        }

        public static ICollection<CollaborationTableValuedModel> GetCheckedOuts(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder));

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_CheckedOutCollaborations,
                queryParameters.ToArray());
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static int CountCheckedOuts(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, usePagination: false));

            int countResults = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Collaboration_CountCheckedOutCollaborations,
                queryParameters.ToArray());
            return countResults;
        }

        public static ICollection<CollaborationTableValuedModel> GetRegistered(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, needDateRange: true));

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_RegisteredCollaborations,
                queryParameters.ToArray());
            return results.OrderByDescending(o => o.PublicationDate).ToList();
        }

        public static int CountRegistered(this IRepositoryAsync<Collaboration> repository, string userName,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, needDateRange: true, usePagination: false));

            int countResults = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Collaboration_CountRegisteredCollaborations,
                queryParameters.ToArray());
            return countResults;
        }

        public static ICollection<CollaborationTableValuedModel> GetProtocolAdmissions(this IRepositoryAsync<Collaboration> repository, string user,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, user)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder));

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_ProtocolAdmissionCollaborations,
                queryParameters.ToArray());
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static int CountProtocolAdmissions(this IRepositoryAsync<Collaboration> repository, string user,
            CollaborationFinderModel collaborationFinder)
        {
            List<QueryParameter> queryParameters = new List<QueryParameter>
            {
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, user)
            };
            queryParameters.AddRange(GetFilterParameters(collaborationFinder, usePagination: false));

            int countResults = repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Collaboration_CountProtocolAdmissionCollaborations,
                queryParameters.ToArray());
            return countResults;
        }

        public static IQueryable<Collaboration> GetByProtocol(this IRepositoryAsync<Collaboration> repository, short year, int number)
        {
            return repository.Query(x => x.Year == year && x.Number == number && x.DocumentType == CollaborationDocumentType.Protocol, true).SelectAsQueryable();
        }

        public static IQueryable<Collaboration> GetByUniqueId(this IRepository<Collaboration> repository, Guid uniqueId)
        {
            IQueryable<Collaboration> result = repository
                .Query(x => x.UniqueId == uniqueId)
                .SelectAsQueryable();
            return result;
        }

        public static IQueryable<Collaboration> GetByUniqueIdWithVersioningAndSign(this IRepository<Collaboration> repository, Guid uniqueId)
        {
            IQueryable<Collaboration> result = repository
                .Query(x => x.UniqueId == uniqueId)
                .Include(i => i.CollaborationSigns)
                .Include(i => i.CollaborationVersionings)
                .SelectAsQueryable();
            return result;
        }

        public static IQueryable<Collaboration> GetById(this IRepository<Collaboration> repository, int collaborationId)
        {
            IQueryable<Collaboration> result = repository
                .Query(x => x.EntityId == collaborationId)
                .Include(i => i.CollaborationVersionings)
                .Include(i => i.Location)
                .SelectAsQueryable();
            return result;
        }

        public static bool HasViewableRight(this IRepositoryAsync<Collaboration> repository, string username, int idCollaboration, bool checkSecretary)
        {
            return repository.Query(x => x.EntityId == idCollaboration &&
                                        (x.RegistrationUser == username ||
                                        x.CollaborationSigns.Any(cs => cs.SignUser == username) ||
                                        (checkSecretary && x.CollaborationUsers.Any(cu => cu.Role.RoleUsers.Any(ru => ru.Account == username && ru.Type == RoleUserType.Secretary)))), true)
                .SelectAsQueryable()
                .Any();
        }

        private static List<QueryParameter> GetFilterParameters(CollaborationFinderModel collaborationFinder,
            bool needIsRequired = false, bool needDateRange = false, bool usePagination = true)
        {
            QueryParameter isRequiredParameter = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_IsRequired, DBNull.Value);
            QueryParameter dateFromParameter = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_DateFrom, DBNull.Value);
            QueryParameter dateToParameter = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_DateTo, DBNull.Value);
            QueryParameter entityIdParameter = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_EntityId, DBNull.Value);
            QueryParameter documentTypeParameter = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_DocumentType, DBNull.Value);
            QueryParameter memorandumDateParameter = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_MemorandumDate, DBNull.Value);
            QueryParameter objectParameter = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_Object, DBNull.Value);
            QueryParameter noteParameter = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_Note, DBNull.Value);
            QueryParameter registrationNameParameter = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_RegistrationName, DBNull.Value);

            if (collaborationFinder.EntityId.HasValue)
            {
                entityIdParameter.ParameterValue = collaborationFinder.EntityId.Value;
            }
            else
            {
                if (collaborationFinder.IsRequired.HasValue)
                {
                    isRequiredParameter.ParameterValue = collaborationFinder.IsRequired.Value;
                }
                if (collaborationFinder.DateFrom.HasValue)
                {
                    dateFromParameter.ParameterValue = collaborationFinder.DateFrom.Value;
                }
                if (collaborationFinder.DateTo.HasValue)
                {
                    dateToParameter.ParameterValue = collaborationFinder.DateTo.Value;
                }
                if (!string.IsNullOrEmpty(collaborationFinder.DocumentType))
                {
                    documentTypeParameter.ParameterValue = collaborationFinder.DocumentType;
                }
                if (collaborationFinder.MemorandumDate.HasValue)
                {
                    memorandumDateParameter.ParameterValue = collaborationFinder.MemorandumDate.Value.Date;
                }
                if (!string.IsNullOrEmpty(collaborationFinder.Object))
                {
                    objectParameter.ParameterValue = collaborationFinder.Object;
                }
                if (!string.IsNullOrEmpty(collaborationFinder.Note))
                {
                    noteParameter.ParameterValue = collaborationFinder.Note;
                }
                if (!string.IsNullOrEmpty(collaborationFinder.RegistrationName))
                {
                    registrationNameParameter.ParameterValue = collaborationFinder.RegistrationName;
                }
            }

            List<QueryParameter> queryParameters = new List<QueryParameter>();
            if (needIsRequired)
            {
                queryParameters.Add(isRequiredParameter);
            }
            if (needDateRange)
            {
                queryParameters.AddRange(new List<QueryParameter>
                {
                    dateFromParameter, dateToParameter
                });
            }
            queryParameters.AddRange(new List<QueryParameter>
            {
                entityIdParameter, documentTypeParameter, memorandumDateParameter, objectParameter, noteParameter, registrationNameParameter
            });
            if (usePagination)
            {
                queryParameters.AddRange(new List<QueryParameter>
                {
                    new QueryParameter(CommonDefinition.SQL_Param_Collaboration_Skip, collaborationFinder.Skip),
                    new QueryParameter(CommonDefinition.SQL_Param_Collaboration_Top, collaborationFinder.Top)
                });
            }

            return queryParameters;
        }
    }
}
