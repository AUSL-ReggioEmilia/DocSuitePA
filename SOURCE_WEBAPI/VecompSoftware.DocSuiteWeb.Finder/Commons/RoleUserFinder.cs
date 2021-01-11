using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using RoleUserModel = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.RoleUserModel;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class RoleUserFinder
    {
        private static readonly string[] _signerTypes = new string[] { RoleUserType.Manager, RoleUserType.Vice };
        private static readonly string _secretaryType = RoleUserType.Secretary;

        public static IQueryable<RoleUser> GetAccounts(this IRepository<RoleUser> repository, string userName)
        {
            return repository
                .Query(x => _signerTypes.Contains(x.Type) && (x.Role.RoleUsers.Any(r => r.Type == _secretaryType && r.Account == userName)), true)
                .SelectAsQueryable();
        }

        public static RoleUser GetFirstHierarchySigner(this IRepository<RoleUser> repository, string fullIncrementalPath, DSWEnvironmentType environment)
        {
            string[] tokens = fullIncrementalPath.Split('|');
            List<RoleUser> results = new List<RoleUser>();
            for (int i = tokens.Length - 1; i >= 0; i--)
            {
                results.AddRange(GetByAuthorizationType(repository, RoleUserType.Manager, short.Parse(tokens[i]), environment).ToList());
                if (results.Any())
                {
                    return results.First();
                }
            }
            return null;
        }

        public static IQueryable<RoleUser> GetByAuthorizationType(this IRepository<RoleUser> repository, string roleUserType, short roleId, DSWEnvironmentType environment)
        {
            return GetByAuthorizationType(repository, new List<string>() { roleUserType }, roleId, environment);
        }

        public static IQueryable<RoleUser> GetByAuthorizationType(this IRepository<RoleUser> repository, ICollection<string> roleUserTypes, short roleId, DSWEnvironmentType environment)
        {
            return repository.Queryable(true)
                .Where(f => f.Role.EntityShortId == roleId && f.Enabled.HasValue && f.Enabled.Value == true && f.DSWEnvironment == environment
                    && f.IsMainRole.HasValue && f.IsMainRole == true)
                .Where(f => roleUserTypes.Contains(f.Type.ToUpper()));
        }

        public static IQueryable<RoleUser> GetInvalidatedRoleUser(this IRepository<RoleUser> repository, bool optimization = true)
        {
            return repository.Query(x => x.Role.TemplateCollaborationUsers.Any(t => t.IsValid == false), optimization).SelectAsQueryable();
        }

        public static bool IsProcedureSecretary(this IRepository<RoleUser> repository, string username, string domain, short idCategory)
        {
            return repository.ExecuteModelScalarFunction<bool>(CommonDefinition.SQL_FX_Fascicle_IsProcedureSecretary,
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_UserName, username), new QueryParameter(CommonDefinition.SQL_Param_Fascicle_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Fascicle_IdCategory, idCategory));
        }

        public static ICollection<SecurityRight> GetUserRights(this IRepository<RoleUser> repository, string username, string domain, bool roleGroupPECRightEnabled)
        {
            ICollection<SecurityRight> results = repository.ExecuteModelFunction<SecurityRight>(CommonDefinition.SQL_FX_UserDomain_UserRights,
                new QueryParameter(CommonDefinition.SQL_Param_UserDomain_UserName, username), 
                new QueryParameter(CommonDefinition.SQL_Param_UserDomain_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_UserDomain_RoleGroupPECRightEnabled, roleGroupPECRightEnabled));
            if (!results.Any(f=> f.Environment == Model.Entities.Commons.DSWEnvironmentType.Collaboration))
            {
                results.Add(new SecurityRight()
                {
                    Environment = Model.Entities.Commons.DSWEnvironmentType.Collaboration,
                    HasInsertable = true,
                    HasSecretaryRole = false,
                    HasSignerRole = false,
                });
            }
            if (!results.Any(f => f.Environment == Model.Entities.Commons.DSWEnvironmentType.Desk))
            {
                results.Add(new SecurityRight()
                {
                    Environment = Model.Entities.Commons.DSWEnvironmentType.Desk,
                    HasInsertable = false
                });
            }
            if (!results.Any(f => f.Environment == Model.Entities.Commons.DSWEnvironmentType.DocumentSeries))
            {
                results.Add(new SecurityRight()
                {
                    Environment = Model.Entities.Commons.DSWEnvironmentType.DocumentSeries,
                    HasInsertable = false,
                    HasViewable = false,
                });
            }
            if (!results.Any(f => f.Environment == Model.Entities.Commons.DSWEnvironmentType.Dossier))
            {
                results.Add(new SecurityRight()
                {
                    Environment = Model.Entities.Commons.DSWEnvironmentType.Dossier,
                    HasInsertable = false,
                    HasViewable = false,
                });
            }
            if (!results.Any(f => f.Environment == Model.Entities.Commons.DSWEnvironmentType.Fascicle))
            {
                results.Add(new SecurityRight()
                {
                    Environment = Model.Entities.Commons.DSWEnvironmentType.Fascicle,
                    HasInsertable = false,
                    HasViewable = false,
                    HasFascicleResponsibleRole = false,
                    HasFascicleSecretaryRole = false
                });
            }
            if (!results.Any(f => f.Environment == Model.Entities.Commons.DSWEnvironmentType.PECMail))
            {
                results.Add(new SecurityRight()
                {
                    Environment = Model.Entities.Commons.DSWEnvironmentType.PECMail,
                    HasInsertable = false,
                    HasViewable = false,
                });
            }
            if (!results.Any(f => f.Environment == Model.Entities.Commons.DSWEnvironmentType.Protocol))
            {
                results.Add(new SecurityRight()
                {
                    Environment = Model.Entities.Commons.DSWEnvironmentType.Protocol,
                    HasInsertable = false,
                    HasViewable = false,
                });
            }
            if (!results.Any(f => f.Environment == Model.Entities.Commons.DSWEnvironmentType.Resolution))
            {
                results.Add(new SecurityRight()
                {
                    Environment = Model.Entities.Commons.DSWEnvironmentType.Resolution,
                    HasInsertable = false,
                    HasViewable = false,
                });
            }
            if (!results.Any(f => f.Environment == Model.Entities.Commons.DSWEnvironmentType.UDS))
            {
                results.Add(new SecurityRight()
                {
                    Environment = Model.Entities.Commons.DSWEnvironmentType.UDS,
                    HasInsertable = false,
                    HasViewable = false,
                });
            }
            return results;
        }
  
        public static ICollection<RoleUserModel> GetRoleUsersFromDossier(this IRepository<RoleUser> repository, Guid? idDossier)
        {
            QueryParameter idDossierParameter = new QueryParameter(CommonDefinition.SQL_Param_UserRole_IdDossier, idDossier);

            return repository.ExecuteModelFunction<RoleUserModel>(
                CommonDefinition.SQL_FX_RoleUser_AllSecretariesFromDossier,
                idDossierParameter);
        }
    }
}
