﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Security.Microsoft
{
    [LogCategory(LogCategoryDefinition.SECURITY)]
    public class ActiveDirectory : ISecurity
    {
        #region [ Fields ]
        private readonly string _anonymous_apiUserName = "localmachine\\anonymous_api";
        private static IEnumerable<LogCategory> _logCategories = null;
        private const string _ldapAddress = "LDAP://"; 
        private const string _defaultNamingContext = "defaultNamingContext";
        private readonly ILogger _logger;
        private readonly ICurrentIdentity _currentIdentity;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private readonly Guid _instanceId;
        private readonly string _domainName;
        private readonly string _distinguishedName;
        private IReadOnlyCollection<DomainGroupModel> _groups = null;
        private IReadOnlyCollection<string> _groupNames = null;
        private string _currentUserEmail;
        private static readonly ConcurrentDictionary<string, DomainUserModel> _cache_getCurrentUser = new ConcurrentDictionary<string, DomainUserModel>();
        private static readonly ConcurrentDictionary<string, ICollection<DomainGroupModel>> _cache_getGroupsCurrentUser = new ConcurrentDictionary<string, ICollection<DomainGroupModel>>();
        private static readonly ConcurrentDictionary<string, ICollection<DomainGroupModel>> _cache_getGroupsFromUser = new ConcurrentDictionary<string, ICollection<DomainGroupModel>>();
        private static readonly ConcurrentDictionary<string, ICollection<DomainUserModel>> _cache_getMembers = new ConcurrentDictionary<string, ICollection<DomainUserModel>>();
        private static readonly ConcurrentDictionary<string, ICollection<DomainGroupModel>> _cache_groupsFinder = new ConcurrentDictionary<string, ICollection<DomainGroupModel>>();
        private static readonly ConcurrentDictionary<string, ICollection<DomainUserModel>> _cache_usersFinder = new ConcurrentDictionary<string, ICollection<DomainUserModel>>();
        
        #region ADProperties
        private const string MAIL = "mail";
        private const string DISTINGUISHED_NAME = "distinguishedname";
        private const string DISPLAY_NAME = "displayname";
        private const string SID = "sid";
        private const string SAMACCOUNTNAME = "samaccountname";
        #endregion

        #endregion

        #region [ Properties ]

        public IReadOnlyCollection<DomainGroupModel> CurrentUserGroups
        {
            get
            {
                if (_groups == null)
                {
                    _groups = GetGroupsCurrentUser();
                }
                return _groups;
            }
        }

        public string CurrentUserEmail
        {
            get
            {
                if (string.IsNullOrEmpty(_currentUserEmail))
                {
                    _currentUserEmail = GetCurrentUser().EmailAddress;
                }
                return _currentUserEmail;
            }
        }

        public IReadOnlyCollection<string> CurrentUserGroupNames
        {
            get
            {
                if (_groupNames == null)
                {
                    _groupNames = CurrentUserGroups.Select(f => f.Name).OrderBy(f => f).ToList();
                }
                return _groupNames;
            }
        }

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ActiveDirectory));
                }
                return _logCategories;
            }
        }

        #endregion

        #region [ Constructor ]

        public ActiveDirectory(ILogger logger, ICurrentIdentity currentIdentity, IMapperUnitOfWork mapperUnitOfWork, IDecryptedParameterEnvService parameterEnvService) //inserire intertfawccia per il get dell'httprequest)
        {
            _logger = logger;
            _currentIdentity = currentIdentity;
            _mapperUnitOfWork = mapperUnitOfWork;
            _parameterEnvService = parameterEnvService;
            _instanceId = Guid.NewGuid();
            try
            {
                _domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                _distinguishedName = string.Empty;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException("Critical error in ActiveDirectory costructor : Get RootDirEntry", ex, DSWExceptionCode.SC_Parameters);
            }

        }

        #endregion

        #region [ Methods ]
        /// <summary>
        /// Ritorna informazione sull'utente specificato
        /// </summary>
        /// <param name="fullUserName"> Nome account nel formato [domain\account]</param>
        /// <returns></returns>
        public DomainUserModel GetUser(string fullUserName)
        {
            return ActionHelper.TryUserPrincipalCatchWithLogger((d, x) =>
            {
                DomainUserModel user = _mapperUnitOfWork.Repository<IDomainUserModelMapper>().Map(x, new DomainUserModel(), d);
                try
                {
                    user.DomainGroups = _mapperUnitOfWork.Repository<IDomainGroupModelMapper>().MapCollection(x.GetAuthorizationGroups());
                }
                catch { }

                return user;

            }, _logger, _parameterEnvService, fullUserName, _cache_getCurrentUser, LogCategories);
        }

        public DomainUserModel GetCurrentUser()
        {
            if (_currentIdentity.FullUserName.Equals(_anonymous_apiUserName))
            {
                return new DomainUserModel()
                {
                    DisplayName = "anonymous_api",
                    DistinguishedName = "anonymous_api",
                    Description = "anonymous_api",
                    Name = "anonymous_api",
                    Surname = "anonymous_api",
                    EmailAddress = "anonymous_api@local.it",
                    Domain = "localmachine",
                    GUID = Guid.NewGuid()
                };
            }
            return GetUser(_currentIdentity.FullUserName);
        }

        public IReadOnlyCollection<DomainGroupModel> GetGroupsCurrentUser()
        {
            ICollection<DomainGroupModel> results = ActionHelper.TryUserPrincipalCatchWithLogger((d, x) => _mapperUnitOfWork.Repository<IDomainGroupModelMapper>().MapCollection(x.GetGroups()),
                _logger, _parameterEnvService, _currentIdentity.FullUserName, _cache_getGroupsCurrentUser, LogCategories) ?? new List<DomainGroupModel>();
            return new ReadOnlyCollection<DomainGroupModel>(results.OrderBy(f => f.DisplayName).ToList());
        }

        public IReadOnlyCollection<DomainGroupModel> GetGroupsFromUser(string fullUserName)
        {
            ICollection<DomainGroupModel> results = ActionHelper.TryUserPrincipalCatchWithLogger((d, x) => _mapperUnitOfWork.Repository<IDomainGroupModelMapper>().MapCollection(x.GetGroups()),
                _logger, _parameterEnvService, fullUserName, _cache_getGroupsFromUser, LogCategories) ?? new List<DomainGroupModel>();
            return new ReadOnlyCollection<DomainGroupModel>(results.OrderBy(f => f.DisplayName).ToList());
        }

        public IReadOnlyCollection<DomainUserModel> GetMembers(string groupName)
        {
            ICollection<DomainUserModel> results = ActionHelper.TryGroupPrincipalCatchWithLogger((d, x) => _mapperUnitOfWork.Repository<IDomainUserModelMapper>()
                    .MapCollection(x.GetMembers(false).OfType<UserPrincipal>().Where(s => s.Enabled.HasValue && s.Enabled.Value), d),
                _logger, _parameterEnvService, groupName, _cache_getMembers, LogCategories) ?? new List<DomainUserModel>();
            return new ReadOnlyCollection<DomainUserModel>(results.OrderBy(f => f.DisplayName).ToList());
        }

        public IReadOnlyCollection<DomainGroupModel> GroupsFinder(string text)
        {
            ICollection<DomainGroupModel> results = ActionHelper.TryGenericCatchWithLogger((d, ctx) =>
            {
                ICollection<DomainGroupModel> result = null;
                if (_cache_groupsFinder.ContainsKey(text) && _cache_groupsFinder.TryGetValue(text, out result))
                {
                    return result;
                }

                string searchString = string.Format("*{0}*", text);
                using (GroupPrincipal searchMaskSamAccountName = new GroupPrincipal(ctx) { SamAccountName = searchString })
                using (PrincipalSearcher searcherSamAccountName = new PrincipalSearcher(searchMaskSamAccountName))
                using (PrincipalSearchResult<Principal> searchResults = searcherSamAccountName.FindAll())
                {
                    results = _mapperUnitOfWork.Repository<IDomainGroupModelMapper>().MapCollection(searchResults.OfType<GroupPrincipal>());
                    _cache_groupsFinder.TryAdd(text, result);
                    return result;
                }
            }, _logger, _parameterEnvService, LogCategories);
            return new ReadOnlyCollection<DomainGroupModel>(results.OrderBy(f => f.DisplayName).ToList());
        }
      
        public IReadOnlyCollection<DomainUserModel> UsersFinder(string text)
        {
            ICollection<DomainUserModel> results = ActionHelper.TryGenericCatchWithLogger((contexts) =>
            {
                ICollection<DomainUserModel> result;
                if (_cache_usersFinder.ContainsKey(text) && _cache_usersFinder.TryGetValue(text, out result))
                {
                    return result;
                }

                List<DomainUserModel> localResults = new List<DomainUserModel>();
                
                string queryString = string.Format(_parameterEnvService.BasicPersonSearcherKey, text);

                foreach (KeyValuePair<string, PrincipalContext> context in contexts)
                {
                    TenantModel tenantModel = _parameterEnvService.TenantModels.SingleOrDefault(f => f.DomainName.Equals(context.Value.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (tenantModel == null)
                    {
                        throw new ArgumentException($"Tenant {context.Value.Name} from {context.Value.UserName} user has not been configurated in TenantModel");
                    }

                    DirectoryEntry entry = new DirectoryEntry($"{_ldapAddress}{context.Value.Name}" , context.Value.UserName, tenantModel.DomainPassword);
                    DirectorySearcher search = new DirectorySearcher(entry, queryString);
                    
                    search.PropertiesToLoad.AddRange(new string[] { MAIL, DISTINGUISHED_NAME, DISPLAY_NAME, SID, SAMACCOUNTNAME });
                    
                    SearchResultCollection searhResults = search.FindAll();
                    foreach (SearchResult item in searhResults)
                    {
                        DirectoryEntry user = item.GetDirectoryEntry();
                        if (user != null)
                        {
                            user.RefreshCache(new string[] { MAIL, DISTINGUISHED_NAME, DISPLAY_NAME, SID, SAMACCOUNTNAME });

                            localResults.Add(_mapperUnitOfWork.Repository<IDomainUserModelMapper>().Map(user, context.Key));
                        }
                    }
                    context.Value.Dispose();
                }
                _cache_usersFinder.TryAdd(text, localResults);
                return localResults;
            }, _logger, _parameterEnvService, LogCategories);
            return new ReadOnlyCollection<DomainUserModel>(results.OrderBy(f => f.FullAccountInformation).ToList());
        }
        #endregion

    }
}
