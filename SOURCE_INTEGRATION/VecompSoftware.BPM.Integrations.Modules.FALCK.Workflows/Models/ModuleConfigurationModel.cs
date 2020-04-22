using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Models
{
    public class ModuleConfigurationModel
    {
        public string ConnectionString { get; set; }
        public string UDSTopic { get; set; }
        public string WorkflowCompletedTopic { get; set; }
        public string UDSSubscriptionSuccess { get; set; }
        public string UDSSubscriptionError { get; set; }
        public string ArchiveName_RDA { get; set; }
        public string ArchiveName_ODA { get; set; }
        public string ArchiveName_VendorInvoice { get; set; }
        public string ArchiveName_VendorCreditMemo { get; set; }
        public string ArchiveName_CustomerInvoice { get; set; }
        public string ArchiveName_CustomerCreditMemo { get; set; }
        public int Contact_RootId { get; set; }
        public string WorkflowTopic { get; set; }
        public string WorkflowSubscriptionSuccess { get; set; }
        public string ExternalViewerBaseUrl { get; set; }
        public string WorkflowStarterName { get; set; }
        public string AttachmentsPath { get; set; }
        public string DropsPath { get; set; }
        public string GeneralConditionFileName { get; set; }
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string CollaborationBiblosArchiveName { get; set; }
        public double LimitPCLCY { get; set; }
        public double LimitLCY { get; set; }
        public double LimitProcurementResponsibleLCY { get; set; }
        public string Country { get; set; }
        public ICollection<string> ValidWorkflows { get; set; }
    }
}
