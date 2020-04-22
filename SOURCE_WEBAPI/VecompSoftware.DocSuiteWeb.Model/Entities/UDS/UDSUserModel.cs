using System;
using VecompSoftware.DocSuiteWeb.Model.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSUserModel
    {
        public Guid UniqueId { get; set; }
        public AuthorizationRoleType AuthorizationType { get; set; }
        public string Account { get; set; }
        public Guid IdUDS { get; set; }
        public int Environment { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
    }
}
