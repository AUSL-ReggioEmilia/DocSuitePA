using System;
using System.ComponentModel;
using System.Linq;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.DocSeriesExporter
{
    public class DocSeriesExporterParameters : JeepParametersBase
    {
        [Description("URL servizio WSSeries")]
        [CategoryAttribute("Valori di default")]
        public string WsSeriesUrl { get; set; }

        [Description("Definisce il tipo Repository dove salvare i CSV creati.")]
        [Category("Valori di default")]
        public RepositoryType Repository { get; set; }

        [Description("Se 1 effettua l'autenticazione con Username e Password.")]
        [Category("Sicurezza")]        
        public string ImpersonateUser { get; set; }

        [Description("Percorso dove salvare i CSV creati su FileSystem. Attivo se selezionata la tipologia di repository FileSystem.")]
        [Category("FileSystem")]
        public string FileSystemPath { get; set; }

        [Description("Url alla Site Collection della Document Library di Destinazione.")]
        [Category("Sharepoint")]
        public string SpSiteCollection { get; set; }

        [Description("Document Library utilizzata per il salvataggio dei CSV su Sharepoint")]
        [Category("Sharepoint")]
        public string SpDocumentLibrary { get; set; }

        [Description("User utilizzato per l'autenticazione a Sharepoint.")]
        [Category("Sharepoint")]
        public string SpUser { get; set; }

        [Description("Password utilizzata per l'autenticazione a Sharepoint.")]
        [Category("Sharepoint")]
        public string SpPassword { get; set; }

        [Description("Domain utilizzato per l'autenticazione a Sharepoint.")]
        [Category("Sharepoint")]
        [DefaultValue("Server")]
        public string SpDomain { get; set; }

        [Description("Indica se la procedura debba esportare in CSV anche l'indicazione dei documenti pubblicati.")]
        [Category("Valori di default")]
        [DefaultValue(false)]
        public bool ExportDocuments { get; set; }
    }
}
