namespace VecompSoftware.DocSuiteWeb.Entity.PECMails
{
    public enum PECMailProcessStatus : int
    {
        Exists = 10,
        Downloaded = 20,
        Processed = 30,
        Archived = 40,
        ArchivedInDocSuiteNext = 1000,
        StoredInDocumentManager = 1001
    }
}
