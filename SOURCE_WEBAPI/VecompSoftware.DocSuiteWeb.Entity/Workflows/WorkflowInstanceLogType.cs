using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{
    public enum WorkflowInstanceLogType : short
    {

        [Description("Documento approvato")]
        DocumentApproved = 0,
        [Description("Documento rifiutato")]
        DocumentRefused = 1,
        [Description("Documento in approvazione")]
        DocumentToApproved = 2,
        [Description("Documento protocollato")]
        DocumentRegistered = 3,
        [Description("Archivio creato")]
        UDSCreated = 4,
        [Description("Archivio protocollato")]
        UDSRegistered = 5,
        [Description("PEC inviata")]
        PECSended = 6,
        [Description("Mail inviata")]
        MailSended = 7,
        [Description("Informazione")]
        Information = 8,
        [Description(" Workflow avviato")]
        WFStarted = 9,
        [Description(" Settore assegnato")]
        WFRoleAssigned = 10,
        [Description(" Presa in carico workflow")]
        WFTakeCharge = 11,
        [Description(" Workflow rilasciato")]
        WFRelease = 12,
        [Description("Workflow rifiutato")]
        WFRefused = 13,
        [Description("Workflow completato")]
        WFCompleted = 14
    }
}