using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Security.Microsoft
{
    [LogCategory(LogCategoryDefinition.SECURITY)]
    public class AzureActiveDirectory : ISecurity
    {
        #region [ Fields ]
        private readonly string _anonymous_apiUserName = "localmachine\\anonymous_api";
        private readonly ILogger _logger;
        private readonly ICurrentIdentity _currentIdentity;
        private string _currentUserEmail;
        #endregion

        #region [ Properties ]
        public IReadOnlyCollection<DomainGroupModel> CurrentUserGroups
        {
            get
            {
                return new List<DomainGroupModel>();
            }

        }

        public IReadOnlyCollection<string> CurrentUserGroupNames
        {
            get
            {
                return new List<string>();
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
        #endregion

        #region [ Constructor ]
        public AzureActiveDirectory(ILogger logger, ICurrentIdentity currentIdentity)
        {
            _logger = logger;
            _currentIdentity = currentIdentity;
        }
        #endregion

        #region [ Methods ]
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

        public DomainUserModel GetUser(string fullUserName)
        {
            return new DomainUserModel()
            {
                DisplayName = _currentIdentity.FullUserName,
                DistinguishedName = _currentIdentity.FullUserName,
                Description = _currentIdentity.FullUserName,
                Name = _currentIdentity.FullUserName,
                Surname = _currentIdentity.FullUserName,
                EmailAddress = _currentIdentity.FullUserName,
                Domain = "AzureAD",
                GUID = Guid.NewGuid()
            };
        }

        public IReadOnlyCollection<DomainGroupModel> GetGroupsCurrentUser()
        {
            return new List<DomainGroupModel>();
        }

        public IReadOnlyCollection<DomainGroupModel> GetGroupsFromUser(string fullUserName)
        {
            return new List<DomainGroupModel>();
        }

        public IReadOnlyCollection<DomainUserModel> GetMembers(string groupName)
        {
            return new List<DomainUserModel>();
        }

        public IReadOnlyCollection<DomainGroupModel> GroupsFinder(string text)
        {
            return new List<DomainGroupModel>();
        }

        public IReadOnlyCollection<DomainUserModel> UsersFinder(string text)
        {
            return new List<DomainUserModel>();
        }
        #endregion
    }
}
