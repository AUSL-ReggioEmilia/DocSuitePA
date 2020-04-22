namespace VecompSoftware.JeepService.PosteWeb.Service
{
    internal class RichiestaStatiLavorazione
    {
        /// <summary> In attesa di stampa. </summary>
        /// <remarks> Transitorio. </remarks>
        public const string AccettataOnline = "00";

        /// <summary> Recapitato al destinatario. </summary>
        /// <remarks> Definitivo. </remarks>
        public const string Consegnato = "01";

        /// <summary> In attesa di essere consegnato. </summary>
        /// <remarks> Transitorio. </remarks>
        public const string Giacienza = "02";

        /// <summary> In ritorno o reso al mittente. </summary>
        /// <remarks> Definitivo. </remarks>
        public const string InRestituzione = "03";

        /// <summary> In consegna. </summary>
        /// <remarks> Transitorio. </remarks>
        public const string InLavorazione = "99";
    }
}