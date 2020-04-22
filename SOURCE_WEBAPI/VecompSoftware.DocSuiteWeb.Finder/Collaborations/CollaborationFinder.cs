using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
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
        public static ICollection<CollaborationTableValuedModel> GetProposed(this IRepositoryAsync<Collaboration> repository, string userName)
        {
            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_ProposedCollaborations,
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName));
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static ICollection<CollaborationTableValuedModel> GetSigning(this IRepositoryAsync<Collaboration> repository, string userName, bool? isRequired)
        {
            QueryParameter requiredParam = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_IsRequired, isRequired);
            if (!isRequired.HasValue)
            {
                requiredParam = new QueryParameter(CommonDefinition.SQL_Param_Collaboration_IsRequired, DBNull.Value);
            }

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_CollaborationsSigning,
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName), requiredParam);
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static ICollection<CollaborationTableValuedModel> GetAllUsers(this IRepositoryAsync<Collaboration> repository, string userName)
        {
            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_AllUserCollaborations,
            new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName));
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static ICollection<CollaborationTableValuedModel> GetActiveUsers(this IRepositoryAsync<Collaboration> repository, string userName, ICollection<string> signers)
        {
            DataTable table = new DataTable();
            table.Columns.Add("val", typeof(string));
            foreach (string sign in signers)
            {
                table.Rows.Add(sign);
            }

            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_ActiveUserCollaborations, new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName),
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_Signers, table) { ParameterTypeName = "string_list_tbltype" });
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static ICollection<CollaborationTableValuedModel> GetAlreadySigned(this IRepositoryAsync<Collaboration> repository, string userName)
        {
            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_AlreadySignedCollaborations, new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName));
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static ICollection<CollaborationTableValuedModel> GetManagings(this IRepositoryAsync<Collaboration> repository, string userName)
        {
            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_CollaborationsManaging, new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName));
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static ICollection<CollaborationTableValuedModel> GetCheckedOuts(this IRepositoryAsync<Collaboration> repository, string userName)
        {
            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_CheckedOutCollaborations, new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName));
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static ICollection<CollaborationTableValuedModel> GetRegistered(this IRepositoryAsync<Collaboration> repository, string userName, DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_RegisteredCollaborations,
                new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, userName), new QueryParameter("@DateFrom", dateFrom), new QueryParameter("@DateTo", dateTo));
            return results.OrderByDescending(o => o.PublicationDate).ToList();
        }

        public static ICollection<CollaborationTableValuedModel> GetProtocolAdmissions(this IRepositoryAsync<Collaboration> repository, string user)
        {
            ICollection<CollaborationTableValuedModel> results = repository.ExecuteModelFunction<CollaborationTableValuedModel>(CommonDefinition.SQL_FX_Collaboration_ProtocolAdmissionCollaborations, new QueryParameter(CommonDefinition.SQL_Param_Collaboration_UserName, user));
            return results.OrderByDescending(o => o.LastChangedDate).ToList();
        }

        public static IQueryable<Collaboration> GetByProtocol(this IRepositoryAsync<Collaboration> repository, short year, int number)
        {
            return repository.Query(x => x.Year == year && x.Number == number && x.DocumentType == "P", true).SelectAsQueryable();
        }

        public static IQueryable<Collaboration> GetByUniqueId(this IRepository<Collaboration> repository, Guid uniqueId)
        {
            IQueryable<Collaboration> result = repository
                .Query(x => x.UniqueId == uniqueId)
                .SelectAsQueryable();
            return result;
        }
    }
}
