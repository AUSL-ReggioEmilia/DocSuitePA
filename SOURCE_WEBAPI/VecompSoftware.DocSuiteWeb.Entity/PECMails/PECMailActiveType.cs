namespace VecompSoftware.DocSuiteWeb.Entity.PECMails
{
    public enum PECMailActiveType : byte
    {
        Delete = 0,
        Active = 1,
        Disabled = 2,
        Processing = 3,
        Processed = 4,
        Error = 255
    }
}
