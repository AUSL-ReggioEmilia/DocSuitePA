
namespace VecompSoftware.DocSuiteWeb.Entity.Resolutions
{
    public enum ResolutionStatus : short
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

