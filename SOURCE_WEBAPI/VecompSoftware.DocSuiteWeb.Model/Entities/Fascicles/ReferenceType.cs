using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public enum ReferenceType : short
    {
        [Description("Fascicolazione")]
        Fascicle = 0,
        [Description("Per Riferimento")]
        Reference = 1
    }
}
