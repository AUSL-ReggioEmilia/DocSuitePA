enum DossierFolderStatus {
    InProgress = 1,
    Fascicle = InProgress * 2, 
    FascicleClose = Fascicle * 2,
    Folder = FascicleClose * 2,
    DoAction = Folder * 2
}
export = DossierFolderStatus;