using System.Linq;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Securities
{
    public class DomainUserModelMapper : BaseDomainModelMapper<UserPrincipal, DomainUserModel>, IDomainUserModelMapper
    {
        public override DomainUserModel Map(UserPrincipal entity, DomainUserModel entityTransformed)
        {

            entityTransformed.EmailAddress = entity.EmailAddress;
            entityTransformed.EmployeeId = entity.EmployeeId;
            entityTransformed.GivenName = entity.GivenName;
            entityTransformed.MiddleName = entity.MiddleName;
            entityTransformed.Surname = entity.Surname;
            entityTransformed.VoiceTelephoneNumber = entity.VoiceTelephoneNumber;
            entityTransformed.Domain = string.Empty;

            return base.Map(entity, entityTransformed);
        }

        public ICollection<DomainUserModel> MapCollection(IEnumerable<UserPrincipal> models, string domain)
        {
            List<DomainUserModel> domainUsers = new List<DomainUserModel>();
            return models.Select(f => Map(f, new DomainUserModel(), domain)).ToList();
        }

        public DomainUserModel Map(UserPrincipal entity, DomainUserModel entityTransformed, string domain)
        {
            DomainUserModel result = Map(entity, entityTransformed);
            result.Domain = domain;
            return result;
        }

    }
}
