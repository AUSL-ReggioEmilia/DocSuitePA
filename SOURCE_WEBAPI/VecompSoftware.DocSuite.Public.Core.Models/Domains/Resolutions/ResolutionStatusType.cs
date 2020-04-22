
namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions
{
    public enum ResolutionStatusType : short
    {
        //Temporaneo
        Temporary = -5,
        //Rettificata
        Rectified = -4,
        //Revocata
        Revoked = -3,
        //Annullata
        Annulled = -2,
        //Errata
        Wrong = -1,
        //Attiva
        Active = 0
    }
}

