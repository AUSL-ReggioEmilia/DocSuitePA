namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles
{
    public enum FascicleType : int
    {
        Legacy = -1,
        SubFascicle = 0,
        Procedure = 1,
        Period = Procedure * 2,
        Activity = Period * 2
    }
}
