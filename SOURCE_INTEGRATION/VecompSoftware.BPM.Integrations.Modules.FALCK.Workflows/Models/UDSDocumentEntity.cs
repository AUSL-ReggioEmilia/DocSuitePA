using System;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Models
{
    public class UDSDocumentEntity
    {
        public Guid IdDocument { get; set; }
        public short DocumentType { get; set; }
        public string DocumentName { get; set; }
    }
}
