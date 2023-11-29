enum InsertActionType {
    InsertActivityFascicle = 0,

    ViewProtocolDocument = 1,

    InsertContainer = ViewProtocolDocument * 2,

    DematerialisationLogInsert = InsertContainer * 2,

    InsertPeriodicFascicle = DematerialisationLogInsert * 2,

    SecureDocumentLogInsert = InsertPeriodicFascicle * 2,

    InsertCategory = SecureDocumentLogInsert * 2,

    InsertDossierFolderAssociatedToFascicle = InsertCategory * 2,

    BuildDossierFolders = InsertDossierFolderAssociatedToFascicle * 2,

    AutomaticIntoFascicleDetection = BuildDossierFolders * 2,

    CreateProtocol = AutomaticIntoFascicleDetection * 2,

    DocumentUnitArchived = CreateProtocol * 2,

    ProtocolShared = DocumentUnitArchived * 2,

    CloneProcessFolder = ProtocolShared * 2,

    InsertProcedureFascicle = CloneProcessFolder * 2,

    CreateDocumentSeriesItem = InsertProcedureFascicle * 2,
}

export = InsertActionType;