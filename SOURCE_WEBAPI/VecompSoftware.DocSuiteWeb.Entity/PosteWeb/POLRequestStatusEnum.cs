namespace VecompSoftware.DocSuiteWeb.Entity.PosteWeb
{
    public enum POLRequestStatusEnum : int
    {
        RequestQueued = 0,
        RequestSent = 1,
        NeedConfirm = 2,
        Confirmed = 3,
        Executed = 4,
        Error = -1,
    }
}
