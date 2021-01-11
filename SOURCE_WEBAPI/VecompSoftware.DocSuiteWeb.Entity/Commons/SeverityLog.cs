namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public enum SeverityLog : short
    {
        /// <summary>
        /// Log disabilitato
        /// </summary>
        Off = 1,
        /// <summary>
        /// Errore critico con terminazione del blocco di esecuzione
        /// </summary>
        Fatal = 2,
        /// <summary>
        /// Stato di errore ma senza terminazione del blocco di esecuzione
        /// </summary>
        Error = 2 * Fatal,
        /// <summary>
        /// Stato di warning
        /// </summary>
        Warning = 2 * Error,
        /// <summary>
        /// Stato di log
        /// </summary>
        Info = 2 * Warning,
        /// <summary>
        /// Stato di debug
        /// </summary>
        Debug = 2 * Info,
        /// <summary>
        /// Traccia tutte i messaggi
        /// </summary>
        Trace = 2 * Debug
    }
}
