using System;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSContact
    {
        public Guid UDSContactId { get; set; }

        public Guid UDSId { get; set; }

        public int? IdContact { get; set; }

        public Guid? UniqueIdContact { get; set; }

        public string ContactManual { get; set; }

        public short ContactType { get; set; }

        public string ContactLabel { get; set; }
    }
}
