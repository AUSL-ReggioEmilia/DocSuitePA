namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Tipologia di contatto Mittente/Destinatario
    /// </summary>
    public enum ContactDirectionType : int
    {
        /// <summary>
        /// Codice non definito
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Mittente
        /// </summary>
        Sender = 1,
        /// <summary>
        /// Destinarario
        /// </summary>
        Recipient = 2,
    }
}
