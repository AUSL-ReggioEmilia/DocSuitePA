using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{
    public enum WorkflowInstanceLogType : short
    {

        [Description("Documento Approvato")]
        DocumentApproved = 0,
        [Description("Documento Rifiutato")]
        DocumentRefused = 1,
        [Description("Documento in Approvazione")]
        DocumentToApproved = 2,
        [Description("Documento Protocollato")]
        DocumentRegistered = 3,
        [Description("Archivio Creato")]
        UDSCreated = 4,
        [Description("Archivio Protocollato")]
        UDSRegistered = 5,
        [Description("PEC Inviata")]
        PECSended = 6,
        [Description("Mail Inviata")]
        MailSended = 7,
        [Description("Informazione")]
        Information = 8,
        [Description(" Workflow Avviato")]
        WFStarted = 9,
        [Description(" Settore WF Assegnato")]
        WFRoleAssigned = 10,
        [Description(" Presa in carico Workflow")]
        WFTakeCharge = 11,
        [Description(" Workflow Rilasciato")]
        WFRelease = 12,
        [Description("Workflow Rifiutato")]
        WFRefused = 13,
        [Description("Workflow Completato")]
        WFCompleted = 14
    }
}