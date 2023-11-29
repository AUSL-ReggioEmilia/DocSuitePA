namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    /// <summary>
    ///     This enum describes update properties for entities. These values are used for servicebus filter queries
    /// </summary>
    public enum UpdatedPropertyType : short
    {
        ProcessNameUpdated = 1,
        DossierFolderNameUpdated = 2,
        ProtocolAnnexedDocumentsUpdated = 3,
        ProtocolAttachmentDocumentsUpdated = 4,
        ProtocolDocumentsUpdated = 5,
        ProtocolCategoryUpdated = 6,
        UDSAnnexedDocumentsUpdated = 7,
        UDSAttachmentDocumentsUpdated = 8,
        UDSMainDocumentUpdated = 9,
        UDSSubjectUpdated = 10,
        UDSCategoryUpdated = 11
    }
}
