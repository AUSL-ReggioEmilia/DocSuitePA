namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Tipologie di unità documentarie della DocSuite
    /// </summary>
    public enum DocumentUnitType : int
    {
        /// <summary>
        /// Non definito
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Protocollo
        /// </summary>
        Protocol = 1,
        /// <summary>
        /// Atto
        /// </summary>
        Resolution = 2,
        /// <summary>
        /// Serie documentale
        /// </summary>
        DocumentSeries = Resolution * 2,
        /// <summary>
        /// Archivio / Unità documentaria specifica
        /// </summary>
        Archive = DocumentSeries * 2
    }
}