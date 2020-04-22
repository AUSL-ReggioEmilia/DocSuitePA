using PetaPoco;
using System;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSDocument : UDSBaseEntity<UDSDocument>
    {
        public Guid UDSDocumentId { get; set; }
        public Guid UDSId { get; set; }
        public Guid IdDocument { get; set; }
        public short DocumentType { get; set; }
        public string DocumentName { get; set; }

        [ResultColumn]
        public string DocumentLabel { get; set; }

        public UDSDocument() : base(UDSTableBuilder.UDSDocumentsPK) { }
    }
}
