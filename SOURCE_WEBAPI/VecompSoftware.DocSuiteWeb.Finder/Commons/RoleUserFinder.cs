using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

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

    }
}
