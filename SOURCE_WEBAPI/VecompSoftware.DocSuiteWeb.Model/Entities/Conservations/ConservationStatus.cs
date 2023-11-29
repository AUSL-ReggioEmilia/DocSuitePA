namespace VecompSoftware.DocSuiteWeb.Model.Conservations
{
    public enum ConservationStatus : short
    {
        Ready = 0,
        Conservated = 1,
        InProgress = 2,
        Unconservable = 3,
        Error = 4,
        Discarded = 5
    }
}
