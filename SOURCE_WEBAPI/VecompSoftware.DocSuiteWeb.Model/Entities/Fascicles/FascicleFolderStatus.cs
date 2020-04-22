using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public enum FascicleFolderStatus : short
    {
        [Description("Cartella attiva")]
        Active = 1,
        [Description("Cartella pubblica via internet")]
        Internet = Active * 2
    }
}
