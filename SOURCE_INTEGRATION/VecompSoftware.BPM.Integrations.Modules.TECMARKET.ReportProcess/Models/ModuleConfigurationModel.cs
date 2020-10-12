using System;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.ReportProcess.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string WorkflowRepositoryNormalName { get; set; }
        public string WorkflowRepositoryErrorName { get; set; }
        public string WorkflowEventsPath { get; set; }
        public string BackupPath { get; set; }
        public Guid IdTemplateDocumentRepository { get; set; }
        public string UDSName { get; set; }
        public string ReportPDFFileName { get; set; }
        public string ReportExcelFileName { get; set; }
        public string ReportSubject { get; set; }
    }
}
