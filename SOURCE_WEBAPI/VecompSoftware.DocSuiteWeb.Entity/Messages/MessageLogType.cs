using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Messages
{
    public enum MessageLogType : int
    {
        [Description("Creazione")]
        Created = 1,
        [Description("Lettura")]
        Viewed = 2,
        [Description("Modifica")]
        Edited = 3,
        [Description("Cancellazione")]
        Deleted = 4,
        [Description("Inviata")]
        Sent = 5,
        [Description("Errore")]
        Error = 6
    }
}
