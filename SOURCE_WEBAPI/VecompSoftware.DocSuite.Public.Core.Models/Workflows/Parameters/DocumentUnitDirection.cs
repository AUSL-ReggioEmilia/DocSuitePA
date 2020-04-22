namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Direzione dell'unità documentaria
    /// </summary>
    public enum DocumentUnitDirection : short
    {
        /// <summary>
        /// Ingresso
        /// </summary>
        Inbound = -1,
        /// <summary>
        /// Interno / Tra uffici
        /// </summary>
        Internal = 0,
        /// <summary>
        /// Uscita
        /// </summary>
        Outgoing = 1,
    }

}