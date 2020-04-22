using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.AVCP
{
    public class AVCPParameters : JeepParametersBase
    {
        public const string AVCP_DROP_FOLDER = "AVCP_DROP";

        [Category("Configurazione")]
        [DefaultValue("http://localhost/ws.asmx")]
        public string AVCPServiceUrl { get; set; }

        [Category("Configurazione")]
        [DefaultValue("http://localhost?Tipo=Resl&Azione=Apri&Identificativo={0}")]
        public string DocumentUrlMask { get; set; }

        [Category("Configurazione")]
        [Description("Category di default per le registrazioni in Serie Documentale. Se non valorizzata recupera la Category dalla Resolution.")]
        public int DefaultCategory { get; set; }

        [Category("Configurazione")]
        [DefaultValue(false)]
        [Description("Indica se devono essere gestite tutte le tipologie di provvedimento o solamente quelle specificate dal parametro AVCPResolutionType di parameterenv. (AUSL-PC = true)")]
        public bool FindAllResolutionTypes { get; set; }

        [Category("Metadata AVCP")]
        [DefaultValue("http://trasp.ausl.pc.it/Document?idSeriesItem={0}")]
        public string AVCPDatasetUrlMask { get; set; }

        [Category("Metadata AVCP")]
        [DefaultValue("AVCP")]
        public string AVCPEntePubblicatore { get; set; }

        [Category("Metadata AVCP")]
        [DefaultValue("")]
        public string AVCPLicenza { get; set; }

        [DefaultValue(false)]
        [Category("Feature Flag")]
        [Description("Se attivo invia l'elenco dei servizi ad ogni aggiornamento.")]
        public bool SendAllCategories { get; set; }

        [DefaultValue(false)]
        [Category("Feature Flag")]
        [Description("Se attivo invia le nuove Resolution adottate.")]
        public bool SendNewDocuments { get; set; }

        [DefaultValue(false)]
        [Category("Feature Flag")]
        [Description("Se attivo aggiorna i documenti con i dataset.")]
        public bool UpdateDocuments { get; set; }

        [DefaultValue(false)]
        [Category("Feature Flag")]
        public bool UnpublishEmptyData { get; set; }


        [Description("Data di filtro per ricerca Resolution")]
        [Category("Resolution Filter")]
        public DateTime ResolutionFromDate { get; set; }

        [Description("Data di filtro per ricerca Resolution")]
        [Category("Resolution Filter")]
        public DateTime ResolutionToDate { get; set; }

        [DefaultValue("")]
        [Description("Username dell'utente di impersonificazione")]
        [Category("Serie Documentale")]
        public string Username { get; set; }

        [DefaultValue("")]
        [Description("Cartella contenente file di configurazione per import Excel")]
        [Category("Import EXCEL")]
        public string ExcelConfigFolder { get; set; }

       
    }
}
