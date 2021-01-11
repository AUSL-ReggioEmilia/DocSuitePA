using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.DTO.Fascicles
{
    public enum FascicleRightPosition
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
        Cancel = 5,
        [Description("Chiusura")]
        Close = 6
    }
}
