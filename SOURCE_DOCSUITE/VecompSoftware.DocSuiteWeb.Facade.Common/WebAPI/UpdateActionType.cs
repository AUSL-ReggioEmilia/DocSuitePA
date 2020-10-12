using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
{
    public enum UpdateActionType : int
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

        [Description("TenantRoleAdd")]
        TenantRoleAdd = TenantContainerAdd * 2,

        [Description("TenantContactAdd")]
        TenantContactAdd = TenantRoleAdd * 2
    }
}
