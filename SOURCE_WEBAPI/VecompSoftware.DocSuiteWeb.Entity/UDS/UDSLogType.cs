using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public enum UDSLogType : short
    {
        [Description("Inserimento Archivio")]
        Insert = 0,
        [Description("Modifica Archivio")]
        Modify = 1,
        [Description("Modifica autorizzazione Archivio")]
        AuthorizationModify = Modify * 2,
        [Description("Modifica documento Archivio")]
        DocumentModify = AuthorizationModify * 2,
        [Description("Modifica oggetto Archivio")]
        ObjectModify = DocumentModify * 2,
        [Description("Cancellazione Archivio")]
        Delete = ObjectModify * 2,
        [Description("Visualizzazione documento Archivio")]
        DocumentView = Delete * 2,
        [Description("Visualizzazione Sommario Archivio")]
        SummaryView = DocumentView * 2,
        [Description("Inserimento autorizzazione Archivio")]
        AuthorizationInsert = SummaryView * 2,
        [Description("Rimozione autorizzazione Archivio")]
        AuthorizationDelete = AuthorizationInsert * 2,
    }
}
