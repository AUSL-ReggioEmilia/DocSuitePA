using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Models
{
    public class WorkflowConfigurationModel
    {
        public Guid TenantAOOId { get; set; }
        public Dictionary<XMLModelKind, InvoiceConfiguration> InvoiceTypes { get; set; }
    }
}
