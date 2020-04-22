using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.Pec
{
    public class PecParams : JeepParametersBase
    {
        private List<int> _allowedMailBoxes;
        private List<int> _disAllowedMailBoxes;

        #region [ Properties ]
        [DefaultValue("")]
        [Description("Nome Host modulo pec JeepService")]
        [Category("Host")]
        public string HostName { get; set; }

        [DefaultValue(8)]
        [Description("N° di giorni che delimita il controllo delle PEC rimaste sul server. Deve essere un numero > 7.")]
        [Category("View Old PEC")]
        public int PECOldDay { get; set; }
        
        [DefaultValue("Drop")]
        [Description("Cartella di elaborazione mail ricevute. ATTENZIONE: se è configurato come Watcher le cartelle DEVONO essere diverse da quelle della PEC")]
        [Category("Folders")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string DropFolder { get; set; }

        [DefaultValue("Error")]
        [Description("Cartella di parcheggio mail con problemi di elaborazione. ATTENZIONE: se è configurato come Watcher le cartelle DEVONO essere diverse da quelle della PEC")]
        [Category("Folders")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string ErrorFolder { get; set; }

        [DefaultValue(Direction.InOut)]
        [Description("Indica il tipo di PEC che deve gestire. ATTENZIONE: se è configurato come Watcher le cartelle DEVONO essere diverse da quelle della PEC")]
        [Category("Configuration")]
        public Direction PecDirection { get; set; }

        [Description("Elenco, separato da virgola, degli identificativi delle PECMailBox che deve gestire")]
        [Category("MailBox")]
        public string AllowedMailBoxIDs { get; set; }

        [Browsable(false)]
        public List<int> AllowedMailBoxes
        {
            get
            {
                if (_allowedMailBoxes == null)
                {
                    _allowedMailBoxes = new List<int>();
                    if (!string.IsNullOrEmpty(AllowedMailBoxIDs))
                    {
                        _allowedMailBoxes = AllowedMailBoxIDs
                            .Split(new[] { ',' })
                            .Select(mb => int.Parse(mb)).ToList();
                    }
                }
                return _allowedMailBoxes;
            }
        }

        [Description("Elenco, separato da virgola, degli identificativi delle PECMailBox che non devono essere gestite dal modulo")]
        [Category("MailBox")]
        public string DisallowedMailBoxIDs { get; set; }

        [Browsable(false)]
        public List<int> DisAllowedMailBoxes
        {
            get
            {
                if (_disAllowedMailBoxes == null)
                {
                    _disAllowedMailBoxes = new List<int>();
                    if (!string.IsNullOrEmpty(DisallowedMailBoxIDs))
                    {
                        var mBoxesV = DisallowedMailBoxIDs.Split(new[] { ',' });
                        foreach (var mb in mBoxesV)
                        {
                            _disAllowedMailBoxes.Add(int.Parse(mb));
                        }
                    }
                }
                return _disAllowedMailBoxes;
            }
        }

        
        [DefaultValue(false)]
        [Description("Se attivo, per ogni PEC nelle MailBox di procollazione si analizza il primo allegato per estrarre i metadati")]
        [Category("Configuration")]
        public bool ForwardedAnalyzer { get; set; }

        [DefaultValue(0)]
        [Category("Configuration")]
        [Description("Pausa (in secondi) tra l'invio di due PEC.")]
        public int SendSleep { get; set; }

        [DefaultValue("Temp")]
        [Description("Cartella per elaborazione file temporaneri.")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        [Category("Folders")]
        public string TempFolder { get; set; }

        [DefaultValue("Backup")]
        [Description("Cartella salvataggio PEC rimosse dal Server")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        [Category("Folders")]
        public string DumpFolder { get; set; }

        [Category("Configuration")]
        [DefaultValue(true)]
        [Description("Definisce se devono essere ricaricate le definizioni delle PEC ad ogni ciclo del modulo")]
        public bool PecMailBoxCache { get; set; }

        [Category("Debug")]
        [DefaultValue("emanuele.fabbri@vecompsoftware.it")]
        [Description("Definisce l'indirizzo da utilizzare per inviare qualunque mail/pec in caso di modalità DEBUG attiva.")]
        public string PecOutAddressDebugMode { get; set; }

        [Category("Debug")]
        [DefaultValue(false)]
        [Description("Abilita la modalità debug e pertanto non cancella mai dal server e invia soltanto all'indirizzo \"PecOutAddressDebugMode\"")]
        public bool DebugModeEnabled { get; set; }

        [Category("Configuration")]
        [DefaultValue(false)]
        [Description("Definisce se ignorare la licenza Limilabs")]
        public bool IgnoreLimilabsLicence { get; set; }


        [Category("Recovering Downloaded items")]
        [DefaultValue(false)]
        [Description("Attiva il ricalcolo dell'envelope")]
        public bool RecoverEnvelopeAttachment { get; set; }

        [Category("Recovering Downloaded items")]
        [Description("Definisce la data di inizio da utilizzare per il ricalcolo dell'envelope")]
        public DateTime RecoverEnvelopeAttachmentStartDate { get; set; }

        [Category("Recovering Downloaded items")]
        [Description("Definisce la data di fine da utilizzare per il ricalcolo dell'envelope")]
        public DateTime RecoverEnvelopeAttachmentEndDate { get; set; }

        [Category("Recovering Downloaded items")]
        [Description("Attiva il ricalcolo dell'originalRecipient")]
        [DefaultValue(true)]
        public bool RecoverOriginalRecipient { get; set; }

        [Category("Biblos")]
        [Description("Definisce il numero massimo di caratteri accettati da Biblos per memorizzare un file. Default: 255")]
        [DefaultValue(255)]
        public int BiblosMaxLength { get; set; }

        [DefaultValue(900000)]
        [Category("Configuration")]
        [Description("Questo parametro permetterà di determinare in millisecondi se un file info.xml non è stato spedito tramite Eventi specifici alle WebAPI, dovuto a errori di comunicazione")]
        public int FileSystemWatcherTreshold { get; set; }

        [DefaultValue(420000)]
        [Category("Configuration")]
        [Description("Intervallo (in milisecondi) che viene usato per evitare un punto di fallimento della gestione della comunicazione verso le WebAPI usato per la scansione della cartelle Error del modulo PEC")]
        public int FileSystemWatcherRetryTimer { get; set; }

        #endregion
    }

}