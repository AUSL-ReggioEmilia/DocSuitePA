using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Common.Infrastructures
{
    public enum UpdateActionType : long
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
        FascicleMoveToFolder = PECMailInvoiceTenantCorrection * 2,

        [Description("TenantContainerAdd")]
        TenantContainerAdd = FascicleMoveToFolder * 2,

        [Description("TenantRoleAdd")]
        TenantRoleAdd = TenantContainerAdd * 2,

        [Description("TenantContactAdd")]
        TenantContactAdd = TenantRoleAdd * 2,

        [Description("TenantConfigurationAdd")]
        TenantConfigurationAdd = TenantContactAdd * 2,

        [Description("TenantPECMailBoxAdd")]
        TenantPECMailBoxAdd = TenantConfigurationAdd * 2,

        [Description("TenantWorkflowRepositoryAdd")]
        TenantWorkflowRepositoryAdd = TenantPECMailBoxAdd * 2,

        [Description("TenantContainerRemove")]
        TenantContainerRemove = TenantWorkflowRepositoryAdd * 2,

        [Description("TenantRoleRemove")]
        TenantRoleRemove = TenantContainerRemove * 2,

        [Description("TenantConfigurationRemove")]
        TenantConfigurationRemove = TenantRoleRemove * 2,

        [Description("TenantPECMailBoxRemove")]
        TenantPECMailBoxRemove = TenantConfigurationRemove * 2,

        [Description("TenantWorkflowRepositoryRemove")]
        TenantWorkflowRepositoryRemove = TenantPECMailBoxRemove * 2,

        [Description("TenantContactRemove")]
        TenantContactRemove = TenantWorkflowRepositoryRemove * 2,

        [Description("CollaborationManaged")]
        CollaborationManaged = TenantContactRemove * 2,

        [Description("TenantContainerAddAll")]
        TenantContainerAddAll = CollaborationManaged * 2,

        [Description("TenantContainerRemoveAll")]
        TenantContainerRemoveAll = TenantContainerAddAll * 2,

        [Description("TenantRoleAddAll")]
        TenantRoleAddAll = TenantContainerRemoveAll * 2,

        [Description("TenantRoleRemoveAll")]
        TenantRoleRemoveAll = TenantRoleAddAll * 2,

        [Description("TenantContactAddAll")]
        TenantContactAddAll = TenantRoleRemoveAll * 2,

        [Description("TenantContactRemoveAll")]
        TenantContactRemoveAll = TenantContactAddAll * 2,

        [Description("CloneProcessDetails")]
        CloneProcessDetails = TenantContactRemoveAll * 2,

        [Description("AssociatedProcessDossierFolderToFascicle")]
        AssociatedProcessDossierFolderToFascicle = CloneProcessDetails * 2,

        [Description("ChangeFascicleType")]
        ChangeFascicleType = AssociatedProcessDossierFolderToFascicle * 2
    }
}
