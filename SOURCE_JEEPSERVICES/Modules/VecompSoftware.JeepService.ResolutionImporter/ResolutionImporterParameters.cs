using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.ResolutionImporter
{
    public class ResolutionImporterParameters : JeepParametersBase
    {

        [Category("Configurazione")]
        [Description("Percorso dove spostare i file dopo l'importazione")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string ImportResolutionDropFolder { get; set; }

        [Category("Configurazione")]
        [Description("Percorso dove spostare i file dopo l'importazione")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string ImportResolutionErrorFolder { get; set; }

        [Category("Configurazione")]
        [Description("Percorso dove cercare i file da importare")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string ImportResolutionSourceFolder { get; set; }

        [Category("Configurazione")]
        [Description("Classificatore Atto")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public int ImportResolutionCategoryId { get; set; }

    }
}
