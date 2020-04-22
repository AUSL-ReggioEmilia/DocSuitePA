using System;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Models
{
    public class UDSBaseEntity
    {
        public Guid UDSId { get; set; }
        public Guid IdUDSRepository { get; set; }
        public short IdCategory { get; set; }
    }
}
