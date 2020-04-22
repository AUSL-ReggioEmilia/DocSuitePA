using System;
using VecompSoftware.DocSuiteWeb.Model.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSRoleModel
    {
        public Guid UniqueId { get; set; }
        public AuthorizationRoleType AuthorizationType { get; set; }
        public string AuthorizationLabel { get; set; }
        public Guid IdUDS { get; set; }
        public int IdRole { get; set; }
        public UDSRelationType RelationType { get; set; }
        public int Environment { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
    }
}
