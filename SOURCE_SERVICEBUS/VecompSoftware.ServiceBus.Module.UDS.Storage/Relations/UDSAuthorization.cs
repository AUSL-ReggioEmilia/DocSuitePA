using System;
using VecompSoftware.Helpers.UDS;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSAuthorization
    {
        public Guid UDSAuthorizationId { get; set; }
        public Guid UDSId { get; set; }
        public int IdRole { get; set; }
        public Guid UniqueIdRole { get; set; }
        public string RoleLabel { get; set; }
        public AuthorizationType AuthorizationType { get; set; }
    }
}
