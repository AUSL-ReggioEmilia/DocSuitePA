using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public enum DossierFolderStatus : short
    {
        [Description("Cartella da definire")]
        InProgress = 1,
        [Description("Fascicolo")]
        Fascicle = InProgress * 2,
        [Description("Fascicolo chiuso")]
        FascicleClose = Fascicle * 2,
        [Description("Cartella contenente struttura di sottocartelle")]
        Folder = FascicleClose * 2,
        [Description("Azione")]
        DoAction = Folder * 2
    }
}
