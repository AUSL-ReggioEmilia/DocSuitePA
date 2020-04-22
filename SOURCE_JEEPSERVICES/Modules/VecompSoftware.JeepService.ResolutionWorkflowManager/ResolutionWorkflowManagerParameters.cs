using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.ResolutionWorkflowManager
{
    public class ResolutionWorkflowManagerParameters : JeepParametersBase
    {

        [Description("Percorso della cartella dove vengono salvati i documenti da pubblicare")]
        public string DocumentFolderPath { get; set; }


        [Description("Percorso della cartella dove vengono spostati i documenti andati in errore")]
        public string ErrorFolderPath { get; set; }

    }
}
