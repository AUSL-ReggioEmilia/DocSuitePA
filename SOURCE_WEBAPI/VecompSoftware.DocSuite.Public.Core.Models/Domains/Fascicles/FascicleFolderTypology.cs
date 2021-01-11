using System.ComponentModel;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles
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
