using System.ComponentModel;


namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public enum DeskLogType
    {
        [Description("Inserimento Tavolo")]
        Insert = 1,
        [Description("Modifica Tavolo")]
        Modify = 2,
        [Description("Errore Tavolo")]
        Error = Modify * 2,
        [Description("Check Out Documento")]
        CheckOut = Error * 2,
        [Description("Check In Documento")]
        CheckIn = CheckOut * 2,
        [Description("Approvazione Documento")]
        Approve = CheckIn *2
    }
}
