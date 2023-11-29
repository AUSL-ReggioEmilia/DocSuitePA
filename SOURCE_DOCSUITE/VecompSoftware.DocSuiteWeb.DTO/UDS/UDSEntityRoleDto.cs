using System;
using VecompSoftware.Helpers.UDS;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityRoleDto
    {
        public Guid? UDSAuthorizationId { get; set; }

        public int? IdRole { get; set; }

        public Guid? UniqueId { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }

        public AuthorizationType AuthorizationType { get; set; }

        public AuthorizationInstanceType AuthorizationInstanceType { get; set; }

        public string Username { get; set; }
    }
}
