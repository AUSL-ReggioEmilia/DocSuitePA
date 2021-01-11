using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public enum DossierLogType : int
    {
        [Description("Inserimento dossier")]
        Insert = 0,
        [Description("Modifica dossier")]
        Modify = 1,
        [Description("Autorizzazione dossier")]
        Authorize = Modify * 2,
        [Description("Visualizzazione dossier")]
        View = Authorize * 2,
        [Description("Eliminazione dossier")]
        Delete = View * 2,
        [Description("Chiusura dossier")]
        Close = Delete * 2,
        [Description("Workflow")]
        Workflow = Close * 2,
        [Description("Creazione cartella")]
        FolderInsert = Workflow * 2,
        [Description("Modifica cartella")]
        FolderModify = FolderInsert * 2,
        [Description("Autorizzazione cartella")]
        FolderAuthorize = FolderModify * 2,
        [Description("Rimozione del fascicolo dalla cartella")]
        FolderFascicleRemove = FolderAuthorize * 2,
        [Description("Chiusura cartella")]
        FolderClose = FolderFascicleRemove * 2,
        [Description("Eliminazione cartella")]
        FolderDelete = FolderClose * 2,
        [Description("Storicizzazione della cartella")]
        FolderHystory = FolderDelete * 2,
        [Description("Presa in carico temporanea della cartella")]
        FolderResponsibleChange = FolderHystory * 2,
        [Description("Presa in carico temporanea del dossier")]
        ResponsibleChange = FolderResponsibleChange * 2,
        [Description("Inserimento fascicolo nel dossier")]
        FascicleInsert = ResponsibleChange * 2,
        [Description("Visualizzazione fascicolo")]
        FascicleView = FascicleInsert * 2,
        [Description("Inserimento inserto in dossier")]
        DocumentInsert = FascicleView * 2,
        [Description("Visualizzazione documenti")]
        DocumentView = DocumentInsert * 2,
        [Description("Eliminazione inserto da dossier")]
        DocumentDelete = DocumentView * 2,
        [Description("Errore generico")]
        Error = DocumentDelete * 2
    }
}
