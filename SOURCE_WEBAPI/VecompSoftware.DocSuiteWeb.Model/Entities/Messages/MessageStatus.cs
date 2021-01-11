using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Messages
{
    public enum MessageStatus : int
    {
        [Description("Bozza")]
        Draft = -100,
        [Description("Invio in corso")]
        Active = 0,
        [Description("Inviato")]
        Sent = 1,
        [Description("Errore")]
        Error = -10
    }
}
