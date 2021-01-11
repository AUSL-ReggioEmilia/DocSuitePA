using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{
    public enum DeskRightPositions
    {
        [Description("Inserimento")]
        Insert = 1,
        [Description("Modifica")]
        Modify = 2,
        [Description("Visualizzazione")]
        ViewDocuments = 3,
        [Description("Sommario")]
        Read = 4,
        [Description("Annullamento")]
        Delete = 5,
        [Description("Chiusura")]
        Close = 6,
        [Description("Collaborazione")]
        Collaboration = 7
    }
}
