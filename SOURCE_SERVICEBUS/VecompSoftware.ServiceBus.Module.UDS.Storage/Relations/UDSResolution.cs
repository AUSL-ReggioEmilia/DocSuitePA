using System;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSResolution
    {
        public Guid UDSResolutionId { get; set; }
        public Guid UDSId { get; set; }
        public int IdResolution { get; set; }
        public Guid UniqueIdResolution { get; set; }
    }
}
