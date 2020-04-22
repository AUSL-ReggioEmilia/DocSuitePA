using System;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSMessage
    {
        public Guid UDSMessageId { get; set; }
        public Guid UDSId { get; set; }
        public int IdMessage { get; set; }
        public Guid UniqueIdMessage { get; set; }
    }
}
