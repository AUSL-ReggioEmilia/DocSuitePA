using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService
{
    public class DSWProtocolJournalParameters : JeepParametersBase
    {
        [Description("Cartella dove generare i pdf dei report")]
        [CategoryAttribute("Parametri ReportViewer")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        [DefaultValue(@"C:\Program Files\Vecomp Software\JeepService8\MODULES\DSWProtocolJournal\Output")]
        public string OutputPath { get; set; }

        [Description("Indirizzo assoluto del template RDLC da utilizzare per generare il report")]
        [CategoryAttribute("Parametri ReportViewer")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        [DefaultValue(@"C:\Program Files\Vecomp Software\JeepService8\MODULES\DSWProtocolJournal\Template\Cliente_RegistroProtocollo.rdlc")]
        public string TemplatePath { get; set; }

        [Description("URL del servizio asmx di applicazione della firma digitale")]
        [CategoryAttribute("Marcatura Temporale e Firma digitale")]
        [DefaultValue("http://%ServerName%/ESignEngine/eSignEngine.asmx")]
        public string SignEngineUrl { get; set; }

        [Description("Nome utente da utilizzare per la marcatura temporale")]
        [CategoryAttribute("Marcatura Temporale e Firma digitale")]
        public string SignEngineUser { get; set; }

        [Description("Password da utilizzare per la marcatura temporale")]
        [CategoryAttribute("Marcatura Temporale e Firma digitale")]
        public string SignEnginePassword { get; set; }

        [Description("Abilita la firma dei registri")]
        [CategoryAttribute("Marcatura Temporale e Firma digitale")]
        [DefaultValue(false)]
        public bool ApplySign { get; set; }

        [Description("Abilita la marcatura temporale dei registri")]
        [CategoryAttribute("Marcatura Temporale e Firma digitale")]
        [DefaultValue(false)]
        public bool ApplyTimeStamp { get; set; }

        [Description("Nome del certificato da utilizzare per effettuare le firme dei registri. Obbligatorio per la firma.")]
        [CategoryAttribute("Marcatura Temporale e Firma digitale")]
        public string CertificateName { get; set; }

        [Description("Definisce il formato infocamere da utilizzare: default è 0")]
        [CategoryAttribute("Marcatura Temporale e Firma digitale")]
        [DefaultValue(0)]
        public int InfoCamereFormat { get; set; }

        [Description("Definisce il limite superato il quale avvisare l'utente di un uso eccessivo di marche temporali")]
        [CategoryAttribute("Manutenzione")]
        [DefaultValue(50)]
        public int TimeStampWarningThreshold { get; set; }

        [Description("Location dove memorizzare i registri")]
        [CategoryAttribute("Biblos")]
        public int Location { get; set; }

        [Description("Formato data da utilizzare per la ricerca")]
        [CategoryAttribute("Biblos")]
        [DefaultValue("dd/MM/yyyy")]
        public string DateFormat { get; set; }

        [Description("Data iniziale dalla quale iniziare a calcolare i registri")]
        [CategoryAttribute("Registro")]
        public DateTime LowerDateLimit { get; set; }

        [Description("Quanti giorni di margine lasciare come data di limite superiore: 1 gg significa che verranno fatti tutti i registri fino a quello di ieri non compreso.")]
        [CategoryAttribute("Registro")]
        [DefaultValue(1)]
        public int UpperDayLimit { get; set; }

        [Description("Numero massimo di registri per giorno")]
        [CategoryAttribute("Registro")]
        [DefaultValue(100)]
        public int BatchDayLimit { get; set; }

        [Description("Rigenera i registri che hanno avuto errori")]
        [CategoryAttribute("Registro")]
        [DefaultValue(false)]
        public bool RestoreBrokenLogs { get; set; }

        [Description("Annulla i protocolli in stato -5 non ancora riparati (li mette in stato -2).")]
        [CategoryAttribute("Registro")]
        [DefaultValue(false)]
        public bool CancelBrokenProtocols { get; set; }

        [Description("Messaggio da inserire nella motivazione di annullamento dei protocolli")]
        [CategoryAttribute("Registro")]
        [DefaultValue("Annullamento registrazione di protocollo incompleta per errore del sistema e non recuperata dall’utente")]
        public string CancelBrokenProtocolMessage { get; set; }
    }
}
