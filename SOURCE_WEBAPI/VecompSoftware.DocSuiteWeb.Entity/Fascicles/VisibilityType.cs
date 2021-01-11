using System.ComponentModel;
namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public enum VisibilityType : short
    {
        [Description("Riservato")]
        Confidential = 0,
        [Description("Pubblico")]
        Accessible = 1
    }
}

