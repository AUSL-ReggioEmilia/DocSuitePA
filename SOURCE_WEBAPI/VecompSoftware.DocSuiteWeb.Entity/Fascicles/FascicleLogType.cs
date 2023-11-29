using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public enum FascicleLogType : int
    {
        [Description("Inserimento fascicolo")]
        Insert = 0,
        [Description("Modifica fascicolo")]
        Modify = 1,
        [Description("Visualizzazione fascicolo")]
        View = Modify * 2,
        [Description("Eliminazione fascicolo")]
        Delete = View * 2,
        [Description("Chiusura fascicolo")]
        Close = Delete * 2,
        [Description("Inserimento unità documentaria nel fascicolo")]
        UDInsert = Close * 2,
        [Description("Inserimento unità documentaria nel fascicolo per riferimento")]
        UDReferenceInsert = UDInsert * 2,
        [Description("Visualizzazione documenti")]
        DocumentView = UDReferenceInsert * 2,
        [Description("Eliminazione unità documentaria del fascicolo")]
        UDDelete = DocumentView * 2,
        [Description("Errore")]
        Error = UDDelete * 2,
        [Description("Inserimento inserto nel fascicolo")]
        DocumentInsert = Error * 2,
        [Description("Eliminazione inserto dal fascicolo")]
        DocumentDelete = DocumentInsert * 2,
        [Description("Workflow")]
        Workflow = DocumentDelete * 2,
        [Description("Autorizzazione fascicolo")]
        Authorize = Workflow * 2,
        [Description("Inserimento cartella")]
        FolderInsert = Authorize * 2,
        [Description("Modifica cartella")]
        FolderUpdate = FolderInsert * 2,
        [Description("Eliminazione cartella")]
        FolderDelete = FolderUpdate * 2,
    }
}
