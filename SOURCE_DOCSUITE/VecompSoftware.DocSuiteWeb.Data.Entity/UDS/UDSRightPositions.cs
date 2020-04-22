using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.UDS
{
    public enum UDSRightPositions
    {
        [Description("Inserimento")]
        Insert = 1,
        [Description("Modifica")]
        Modify = 2,
        [Description("Visualizzazione")]
        ViewDocuments = 3,
        [Description("Sommario")]
        Read = 4,
        [Description("Protocollazione")]
        Protocol = 5,
        [Description("PEC Ingresso")]
        PECIngoing = 6,
        [Description("PEC Uscita")]
        PECOutgoing = 7,
        [Description("Annullamento")]
        Cancel = 8
    }
}
