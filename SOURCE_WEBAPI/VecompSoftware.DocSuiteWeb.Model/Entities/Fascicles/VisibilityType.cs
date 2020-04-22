using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public enum VisibilityType : short
    {
        [Description("Riservato")]
        Confidential = 0,
        [Description("Pubblico")]
        Accessible = 1
    }
}
