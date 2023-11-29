using System.Linq;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using System.DirectoryServices;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Securities
{
    public class DomainUserModelMapper : BaseDomainModelMapper<UserPrincipal, DomainUserModel>, IDomainUserModelMapper
    {
        #region ADProperties
        private const string MAIL = "mail";
        private const string DISTINGUISHED_NAME = "distinguishedname";
        private const string DISPLAY_NAME = "displayname";
        private const string SID = "sid";
        private const string SAMACCOUNTNAME = "samaccountname";
        #endregion

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

        public DomainUserModel Map(DirectoryEntry entity, string domain)
        {
            DomainUserModel result = new DomainUserModel();
            result.EmailAddress = entity.Properties.Contains(MAIL)
                ? entity.Properties[MAIL].Value.ToString()
                : string.Empty;
            result.DistinguishedName = entity.Properties.Contains(DISTINGUISHED_NAME)
                ? entity.Properties[DISTINGUISHED_NAME].Value.ToString()
                : string.Empty;
            result.DisplayName = entity.Properties.Contains(DISPLAY_NAME)
                 ? entity.Properties[DISPLAY_NAME].Value.ToString()
                 : string.Empty;
            result.GUID = entity.Guid;
            result.Name = entity.Properties.Contains(SAMACCOUNTNAME)
                 ? entity.Properties[SAMACCOUNTNAME].Value.ToString()
                 : string.Empty;
            result.SDDL_SID = entity.Properties.Contains(SID)
                ? entity.Properties[SID].Value.ToString()
                : string.Empty;

            result.Domain = domain;
            return result;
        }
    }
}
