using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public enum FascicleFolderTypology : short
    {
        [Description("Root")]
        Root = 0,
        [Description("Fascicolo")]
        Fascicle = 1,
        [Description("Sottofascicolo")]
        SubFascicle = Fascicle * 2
    }
}
