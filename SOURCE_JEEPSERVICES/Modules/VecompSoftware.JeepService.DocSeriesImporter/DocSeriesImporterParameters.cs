using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.DocSeriesImporter
{
    public class DocSeriesImporterParameters : JeepParametersBase
    {
        public const string DropFolderDefault = "DocSeriesImporter_Drop";
        public const string DoneFolderNameDefault = "DocSeriesImporter_Done";
        public const string ErrorFolderDefault = "DocSeriesImporter_Error";

        [DefaultValue(DropFolderDefault)]
        [Description("Cartella di download ed elaborazione dei file")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string DropFolder { get; set; }

        [DefaultValue(DoneFolderNameDefault)]
        [Description("Nome cartella di destinazione file elaborati")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string DoneFolderName { get; set; }

        [DefaultValue(ErrorFolderDefault)]
        [Description("Nome cartella di destinazione file che devono essere riprocessati perchè andati in errore")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string ErrorFolder { get; set; }

        [DefaultValue(10)]
        [Description("Intervallo di elaborazione tra task differenti in secondi")]
        public int SleepSeconds { get; set; }

        [DefaultValue(true)]
        [Description("Permette di scegliere se eliminare o meno il file principale")]
        public bool DeleteDoc_Main { get; set; }
        [DefaultValue(true)]
        [Description("Permette di scegliere se eliminare o meno gli Annessi")]
        public bool DeleteDoc_Annexed { get; set; }
        [DefaultValue(true)]
        [Description("Permette di scegliere se eliminare o meno i file Non Pubblicati")]
        public bool DeleteDoc_Unpulished { get; set; }

        [DefaultValue(3)]
        [Description("Numero massimo di volte che un file in Errore può essere riprocessato")]
        public int MaxTimes_ReWorkError { get; set; }
    }
}
