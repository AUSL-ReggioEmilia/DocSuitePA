using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Common.Infrastructures
{
    public enum InsertActionType : int
    {
        [Description("None")]
        None = -1,

        [Description("InsertActivityFascicle")]
        InsertActivityFascicle = 0,

        [Description("ViewProtocolDocument")]
        ViewProtocolDocument = 1,

        [Description("InsertContainer")]
        InsertContainer = ViewProtocolDocument * 2,

        [Description("DematerialisationLogInsert")]
        DematerialisationLogInsert = InsertContainer * 2,

        [Description("InsertPeriodicFascicle")]
        InsertPeriodicFascicle = DematerialisationLogInsert * 2,

        [Description("SecureDocumentLogInsert")]
        SecureDocumentLogInsert = InsertPeriodicFascicle * 2,

        [Description("InsertCategory")]
        InsertCategory = SecureDocumentLogInsert * 2,

        [Description("InsertDossierFolderAssociatedToFascicle")]
        InsertDossierFolderAssociatedToFascicle = InsertCategory * 2,

        [Description("BuildDossierFolders")]
        BuildDossierFolders = InsertDossierFolderAssociatedToFascicle * 2,

        [Description("AutomaticIntoFascicleDetection")]
        AutomaticIntoFascicleDetection = BuildDossierFolders * 2,

        [Description("CreateProtocol")]
        CreateProtocol = AutomaticIntoFascicleDetection * 2,

        [Description("DocumentUnitArchived")]
        DocumentUnitArchived = CreateProtocol * 2,

        [Description("ProtocolShared")]
        ProtocolShared = DocumentUnitArchived * 2,

        [Description("CloneDossierFolder")]
        CloneProcessDetails = ProtocolShared * 2,

        [Description("InsertProcedureFascicle")]
        InsertProcedureFascicle = CloneProcessDetails * 2
    }
}
