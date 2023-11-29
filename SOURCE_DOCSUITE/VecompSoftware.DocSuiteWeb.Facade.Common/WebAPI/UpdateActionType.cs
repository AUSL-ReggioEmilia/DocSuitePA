using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
{
    public enum UpdateActionType : long
    {
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

        [Description("FREE_01")]
        FREE_01 = TenantContainerAdd * 2,

        [Description("FREE_02")]
        FREE_02 = FREE_01 * 2,

        [Description("TenantConfigurationAdd")]
        TenantConfigurationAdd = FREE_02 * 2,

        [Description("TenantPECMailBoxAdd")]
        TenantPECMailBoxAdd = TenantConfigurationAdd * 2,

        [Description("TenantWorkflowRepositoryAdd")]
        TenantWorkflowRepositoryAdd = TenantPECMailBoxAdd * 2,

        [Description("TenantContainerRemove")]
        TenantContainerRemove = TenantWorkflowRepositoryAdd * 2,

        [Description("FREE_03")]
        FREE_03 = TenantContainerRemove * 2,

        [Description("TenantConfigurationRemove")]
        TenantConfigurationRemove = FREE_03 * 2,

        [Description("TenantPECMailBoxRemove")]
        TenantPECMailBoxRemove = TenantConfigurationRemove * 2,

        [Description("TenantWorkflowRepositoryRemove")]
        TenantWorkflowRepositoryRemove = TenantPECMailBoxRemove * 2,

        [Description("FREE_08")]
        FREE_08 = TenantWorkflowRepositoryRemove * 2,

        [Description("CollaborationManaged")]
        CollaborationManaged = FREE_08 * 2,

        [Description("TenantContainerAddAll")]
        TenantContainerAddAll = CollaborationManaged * 2,

        [Description("TenantContainerRemoveAll")]
        TenantContainerRemoveAll = TenantContainerAddAll * 2,

        [Description("FREE_04")]
        FREE_04 = TenantContainerRemoveAll * 2,

        [Description("FREE_05")]
        FREE_05 = FREE_04 * 2,

        [Description("FREE_06")]
        FREE_06 = FREE_05 * 2,

        [Description("FREE_07")]
        FREE_07 = FREE_06 * 2,

        [Description("CloneProcessDetails")]
        CloneProcessDetails = FREE_07 * 2,

        [Description("AssociatedProcessDossierFolderToFascicle")]
        AssociatedProcessDossierFolderToFascicle = CloneProcessDetails * 2,

        [Description("ChangeFascicleType")]
        ChangeFascicleType = AssociatedProcessDossierFolderToFascicle * 2,

        [Description("PublishDocumentSeriesItem")]
        PublishDocumentSeriesItem = ChangeFascicleType * 2,

        [Description("ActivateDocumentSeriesItem")]
        ActivateDocumentSeriesItem = PublishDocumentSeriesItem * 2
    }
}
