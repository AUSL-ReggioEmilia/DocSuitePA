namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.PECMails
{
    public enum PECActiveType : short
    {
        Delete = 0,
        Active = 1,
        Disabled = 2,
        Processing = 3,
        Processed = 4,
        Error = 255
    }
}
