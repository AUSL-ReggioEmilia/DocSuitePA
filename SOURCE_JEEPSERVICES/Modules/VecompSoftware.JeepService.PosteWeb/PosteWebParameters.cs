using System;
using System.ComponentModel;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.PosteWeb
{
    public class PosteWebParameters : JeepParametersBase
    {
        /// <summary> Se Offline l'applicazione non esegue chiamate al WebService. </summary>
        [DefaultValue(false)]
        [Description("Se Offline l'applicazione non esegue chiamate al WebService.")]
        [Category("Default")]
        public bool OfflineMode { get; set; }

        /// <summary> Se Offline l'applicazione non esegue chiamate al WebService. </summary>
        [DefaultValue(41)]
        [Description("Limite del MaxAddress per il truncate")]
        [Category("Default")]
        public int MaxAddress { get; set; }

        [DefaultValue(false)]
        [Description("Se attivo i destinatari del TLG vengono inseriti nel testo del telegramma")]
        [Category("Opzioni di invio")]
        [DisplayName("Invio al Mittente")]
        public bool SendTOLToSender { get; set; }

        [DefaultValue(true)]
        [Description("Se true indica una stampa in Bianco e Nero. Se false indica una stampa a colori")]
        [Category("Opzioni di stampa")]
        [DisplayName("Stampa Bianco/Nero")]
        public bool PrintBW { get; set; }

        [DefaultValue(true)]
        [Description("Se true indica una stampa Fronte/Retro. Se false indica una stampa normale")]
        [Category("Opzioni di stampa")]
        [DisplayName("Stampa Fronte/Retro")]
        public bool PrintDoubleSided { get; set; }
    }
}
