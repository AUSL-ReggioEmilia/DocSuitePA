using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Security
{
    public interface ISecurity
    {
        IReadOnlyCollection<DomainGroupModel> CurrentUserGroups { get; }
        IReadOnlyCollection<string> CurrentUserGroupNames { get; }
        string CurrentUserEmail { get; }
        DomainUserModel GetUser(string fullUserName);
        DomainUserModel GetCurrentUser();
        IReadOnlyCollection<DomainGroupModel> GetGroupsCurrentUser();
        IReadOnlyCollection<DomainGroupModel> GetGroupsFromUser(string fullUserName);
        IReadOnlyCollection<DomainGroupModel> GroupsFinder(string text);
        IReadOnlyCollection<DomainUserModel> GetMembers(string groupName);
        IReadOnlyCollection<DomainUserModel> UsersFinder(string text);

    }
}