namespace VecompSoftware.DocSuiteWeb.Common.Securities
{
    public interface ICurrentIdentity
    {
        string FullUserName { get; }

        string Account { get; }

        string Domain { get; }
    }
}
