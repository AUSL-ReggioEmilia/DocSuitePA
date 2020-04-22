using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public enum FascicleFolderStatus : short
    {
        [Description("Cartella attiva")]
        Active = 1,
        [Description("Cartella pubblica via internet")]
        Internet = Active * 2
    }
}
