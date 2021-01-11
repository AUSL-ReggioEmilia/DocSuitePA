using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Securities
{
    public interface IDomainUserModelMapper : IDomainMapper<UserPrincipal, DomainUserModel>
    {
        DomainUserModel Map(UserPrincipal entity, DomainUserModel entityTransformed, string domain);
        ICollection<DomainUserModel> MapCollection(IEnumerable<UserPrincipal> userPrincipal, string domain);
    }
}
