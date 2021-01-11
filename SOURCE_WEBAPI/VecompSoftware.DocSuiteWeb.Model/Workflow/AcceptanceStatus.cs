using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public enum AcceptanceStatus
    {
        [Description("Non valido")]
        Invalid = 0,
        [Description("Accettazione")]
        Accepted = 1,
        [Description("Rifiuto")]
        Refused = Accepted * 2
    }
}
