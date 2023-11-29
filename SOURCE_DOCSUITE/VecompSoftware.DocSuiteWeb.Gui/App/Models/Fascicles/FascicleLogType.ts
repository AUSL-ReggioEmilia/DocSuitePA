enum FascicleLogType {
    Insert = 0,
    Modify = 1,
    View = Modify * 2,
    Delete = View * 2,
    Close = Delete * 2,
    UDInsert = Close * 2,
    UDReferenceInsert = UDInsert * 2,
    DocumentView = UDReferenceInsert * 2,
    UDDelete = DocumentView * 2,
    Error = UDDelete * 2,
    DocumentInsert = Error * 2,
    DocumentDelete = DocumentInsert * 2,
    Workflow = DocumentDelete * 2 ,
    Authorize = Workflow * 2,
    FolderInsert = Authorize * 2,
    FolderUpdate = FolderInsert * 2,
    FolderDelete = FolderUpdate * 2,
}

export = FascicleLogType;