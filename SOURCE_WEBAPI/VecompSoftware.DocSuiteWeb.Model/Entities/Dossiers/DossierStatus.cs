using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers
{
    public enum DossierStatus : short
    {
        [Description("Annullato")]
        Canceled = 0,

        [Description("Aperto")]
        Open = 1,

        [Description("Chiuso")]
        Closed = 2
    }
}
