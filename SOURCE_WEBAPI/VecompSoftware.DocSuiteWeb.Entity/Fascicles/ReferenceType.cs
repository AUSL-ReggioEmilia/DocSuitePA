using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public enum ReferenceType : short
    {
        [Description("Fascicolazione")]
        Fascicle = 0,
        [Description("Per riferimento")]
        Reference = 1
    }
}
