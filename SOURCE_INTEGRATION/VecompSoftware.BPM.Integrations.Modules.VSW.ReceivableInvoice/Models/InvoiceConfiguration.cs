using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Models
{
    public class InvoiceConfiguration
    {
        public string WorkflowRepositoryName { get; set; }
        public string UDSRepositoryName { get; set; }
        public short ProtocolContainerId { get; set; }
        public short ProtocolCategoryId { get; set; }
        public int ContactParent { get; set; }
        public List<short> AuthorizationRoles { get; set; }
        public Dictionary<string, InvoiceFilePersistance> InvoicePersistanceConfigurations { get; set; }
    }
}
