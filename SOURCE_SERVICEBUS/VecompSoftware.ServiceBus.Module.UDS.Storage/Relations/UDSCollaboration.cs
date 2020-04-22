using System;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSCollaboration
    {
        public Guid UDSCollaborationId { get; set; }
        public Guid UDSId { get; set; }
        public int IdCollaboration { get; set; }
        public Guid? CollaborationUniqueId { get; set; }
        public string CollaborationTemplateName { get; set; }
    }
}
