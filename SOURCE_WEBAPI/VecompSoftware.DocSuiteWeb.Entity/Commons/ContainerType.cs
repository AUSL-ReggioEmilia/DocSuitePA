namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public enum ContainerType : int
    {
        Protocol = 1,
        Resolution = Protocol * 2,
        Document = Resolution * 2,
        DocumentSeries = Document * 2,
        Desk = DocumentSeries * 2,
        UDS = Desk * 2
    }
}
