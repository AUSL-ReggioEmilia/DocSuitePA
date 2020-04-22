namespace VecompSoftware.DocSuiteWeb.Data.Entity.Commons
{
    public enum LogicalStateType
    {
        /// <summary>
        /// Cancellato
        /// </summary>
        Delete = 0,
        /// <summary>
        /// Attivato
        /// </summary>
        Active = 1,
        /// <summary>
        /// In elaborazione
        /// </summary>
        Processing = 2 * Active,
        /// <summary>
        /// Processato senza attribuzione del risultato stato 
        /// </summary>
        Processed = 2 * Processing,
        /// <summary>
        /// Completato con successo 
        /// </summary>
        Successful = 2 * Processed,
        /// <summary>
        /// Completato con errore
        /// </summary>
        Error = 2 * Successful,
    }
}
