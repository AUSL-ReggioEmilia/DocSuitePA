using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public enum WorkflowActivityAction : short
    {
        [Description("Create")]
        Create = 0,
        [Description("ToProtocol")]
        ToProtocol = 1,
        [Description("ToPEC")]
        ToPEC = 2,
        [Description("ToCollaboration")]
        ToCollaboration = 3,
        [Description("ToDesk")]
        ToDesk = 4,
        [Description("ToResolution")]
        ToResolution = 5,
        [Description("ToSign")]
        ToSign = 6,
        [Description("ToAssignment")]
        ToAssignment = 7,
        [Description("ToDossier")]
        ToDossier = 8,
        [Description("ToFascicle")]
        ToFascicle = 9,
        [Description("ToDocumentUnit")]
        ToDocumentUnit = 10,
        [Description("ToArchive")]
        ToArchive = 11,
        [Description("ToMessage")]
        ToMessage = 12,
        [Description("CancelProtocol")]
        CancelProtocol = 13,
        [Description("CancelArchive")]
        CancelArchive = 14,
        [Description("CancelDocumentUnit")]
        CancelDocumentUnit = 15,
        [Description("ToApprove")]
        ToApprove = 16,
        [Description("ToShare")]
        ToShare = 17,
        [Description("UpdateArchive")]
        UpdateArchive = 18,
        [Description("UpdateFascicle")]
        UpdateFascicle = 19,
        [Description("ToIntegration")]
        ToIntegration = 20,
        [Description("GenerateReport")]
        GenerateReport = 21,
        [Description("CopyFascicleContents")]
        CopyFascicleContents = 22
    }
}
