using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Common.Infrastructures
{
    public enum UpdateActionType : int
    {
        [Description("None")]
        None = -1,

        [Description("FascicleClose")]
        FascicleClose = 0,

        [Description("TemplateCollaborationPublish")]
        TemplateCollaborationPublish = 1,

        [Description("RoleUserTemplateCollaborationInvalid")]
        RoleUserTemplateCollaborationInvalid = 2,

        [Description("ActivityFascicleUpdate")]
        ActivityFascicleUpdate = RoleUserTemplateCollaborationInvalid * 2,

        [Description("ActivityFascicleClose")]
        ActivityFascicleClose = ActivityFascicleUpdate * 2,

        [Description("ProtocolArchivedUpdate")]
        ProtocolArchivedUpdate = ActivityFascicleClose * 2,

        [Description("AutomaticHandlingWorkflow")]
        HandlingWorkflow = ProtocolArchivedUpdate * 2,

        [Description("RelaseHandlingWorkflow")]
        RelaseHandlingWorkflow = HandlingWorkflow * 2,

        [Description("RemoveFascicleFromDossierFolder")]
        RemoveFascicleFromDossierFolder = RelaseHandlingWorkflow * 2,

        [Description("AssociatedFascicleToDossierFolder")]
        AssociatedFascicleToDossierFolder = RemoveFascicleFromDossierFolder * 2,

        [Description("CompleteDematerialisationWorkflow")]
        CompleteDematerialisationWorkflow = AssociatedFascicleToDossierFolder * 2,

        [Description("DossierFolderAuthorizationsPropagation")]
        DossierFolderAuthorizationsPropagation = CompleteDematerialisationWorkflow * 2,

        [Description("CompleteSecureDocumentWorkflow")]
        CompleteSecureDocumentWorkflow = DossierFolderAuthorizationsPropagation * 2,

        [Description("UpdateCategory")]
        UpdateCategory = CompleteSecureDocumentWorkflow * 2,

        [Description("PeriodicFascicleClose")]
        PeriodicFascicleClose = UpdateCategory * 2,

        [Description("ActivateProtocol")]
        ActivateProtocol = PeriodicFascicleClose * 2,

        [Description("OpenFascicleClosed")]
        OpenFascicleClosed = ActivateProtocol * 2,

        [Description("CancelFascicle")]
        CancelFascicle = OpenFascicleClosed * 2,

        [Description("PECMailArchivedUpdate")]
        PECMailManaged = CancelFascicle * 2,

        [Description("PECMailInvoiceTenantCorrection")]
        PECMailInvoiceTenantCorrection = PECMailManaged * 2,

        [Description("FascicleMoveToFolder")]
        FascicleMoveToFolder = PECMailInvoiceTenantCorrection * 2

    }
}
