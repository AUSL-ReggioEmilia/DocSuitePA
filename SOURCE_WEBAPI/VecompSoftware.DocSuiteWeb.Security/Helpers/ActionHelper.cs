using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Security
{
    public static class ActionHelper
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        public static TModel TryUserPrincipalCatchWithLogger<TModel>(Func<string, UserPrincipal, TModel> func, ILogger logger,
            IDecryptedParameterEnvService parameterEnvService, string fullUserName, ConcurrentDictionary<string, TModel> cache_result,
            IEnumerable<LogCategory> logCategories)
        {
            TModel result = default(TModel);
            try
            {

                if (cache_result.ContainsKey(fullUserName) && cache_result.TryGetValue(fullUserName, out result))
                {
                    return result;
                }

                string domain = parameterEnvService.CurrentTenantModel.DomainName;
                string samAccountName = fullUserName;

                if (fullUserName.Contains('\\'))
                {
                    string[] token = fullUserName.Split('\\');
                    domain = token.First();
                    samAccountName = token.Last();
                }
                TenantModel tenantModel = parameterEnvService.TenantModels.SingleOrDefault(f => f.DomainName.Equals(domain, StringComparison.InvariantCultureIgnoreCase));
                if (tenantModel == null)
                {
                    throw new ArgumentException($"Tenant {domain} from {fullUserName} user has not been configurated in TenantModel");
                }
                UserPrincipal user;
                using (PrincipalContext context = new PrincipalContext((ContextType)(int)tenantModel.SecurityContext, tenantModel.DomainAddress, tenantModel.DomainUser, tenantModel.DomainPassword))
                using (UserPrincipal userPrincipal = new UserPrincipal(context))
                {
                    userPrincipal.SamAccountName = samAccountName;
                    using (PrincipalSearcher searcher = new PrincipalSearcher(userPrincipal))
                    {
                        user = searcher.FindOne() as UserPrincipal;
                        if (user == null)
                        {
                            throw new DSWSecurityException($"Account {fullUserName} not found in domain controller {domain}", null, DSWExceptionCode.SC_NotFoundAccount);
                        }
                        result = func(tenantModel.DomainName, user);
                        cache_result.TryAdd(fullUserName, result);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is DSWException)
                {
                    throw;
                }
                logger.WriteWarning(ex, logCategories);
            }
            return result;
        }

        public static TModel TryGroupPrincipalCatchWithLogger<TModel>(Func<string, GroupPrincipal, TModel> func, ILogger logger,
            IDecryptedParameterEnvService parameterEnvService, string groupName, ConcurrentDictionary<string, TModel> cache_result,
            IEnumerable<LogCategory> logCategories)
        {
            TModel result = default(TModel);
            try
            {
                if (cache_result.ContainsKey(groupName) && cache_result.TryGetValue(groupName, out result))
                {
                    return result;
                }
                TenantModel tenantModel = parameterEnvService.CurrentTenantModel;
                GroupPrincipal group;
                using (PrincipalContext context = new PrincipalContext((ContextType)(int)tenantModel.SecurityContext, tenantModel.DomainAddress, tenantModel.DomainUser, tenantModel.DomainPassword))
                using (GroupPrincipal groupPrincipal = new GroupPrincipal(context))
                {
                    groupPrincipal.SamAccountName = groupName;
                    using (PrincipalSearcher searcher = new PrincipalSearcher(groupPrincipal))
                    {
                        group = searcher.FindOne() as GroupPrincipal;
                        if (group == null)
                        {
                            throw new DSWException($"Group {groupName} not found in domain controller {tenantModel.DomainName}", null, DSWExceptionCode.SC_NotFoundAccount);
                        }
                        result = func(tenantModel.DomainName, group);
                        cache_result.TryAdd(groupName, result);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is DSWException)
                {
                    throw;
                }
                logger.WriteWarning(ex, logCategories);
            }
            return result;
        }

        public static TModel TryGenericCatchWithLogger<TModel>(Func<string, PrincipalContext, TModel> func, ILogger logger,
            IDecryptedParameterEnvService parameterEnvService, IEnumerable<LogCategory> logCategories)
        {
            try
            {
                TenantModel tenantModel = parameterEnvService.CurrentTenantModel;
                using (PrincipalContext context = new PrincipalContext((ContextType)(int)tenantModel.SecurityContext, tenantModel.DomainAddress, tenantModel.DomainUser, tenantModel.DomainPassword))
                {
                    return func(tenantModel.DomainName, context);
                }
            }
            catch (Exception ex)
            {
                if (ex is DSWException)
                {
                    throw;
                }
                logger.WriteWarning(ex, logCategories);
            }
            return default(TModel);
        }

        public static TModel TryGenericCatchWithLogger<TModel>(Func<IReadOnlyDictionary<string, PrincipalContext>, TModel> func, ILogger logger,
            IDecryptedParameterEnvService parameterEnvService, IEnumerable<LogCategory> logCategories)
        {
            try
            {
                Dictionary<string, PrincipalContext> contexts = new Dictionary<string, PrincipalContext>();
                foreach (TenantModel tenantModel in parameterEnvService.TenantModels)
                {
                    contexts.Add(tenantModel.DomainName, new PrincipalContext((ContextType)(int)tenantModel.SecurityContext, tenantModel.DomainAddress, tenantModel.DomainUser, tenantModel.DomainPassword));
                }
                return func(contexts);
            }
            catch (Exception ex)
            {
                if (ex is DSWException)
                {
                    throw;
                }
                logger.WriteWarning(ex, logCategories);
            }
            return default(TModel);
        }
        #endregion
    }
}
