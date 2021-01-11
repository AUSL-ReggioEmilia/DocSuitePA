namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{

    /// <summary>
    /// Priorità della collaborazione
    /// </summary>
    public enum CollaborationPriorityType : int
    {
        /// <summary>
        /// Non definito
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Bassa
        /// </summary>
        Low = 1,
        /// <summary>
        /// Normale / Default
        /// </summary>
        Normal = 2,
        /// <summary>
        /// Alta / Urgente
        /// </summary>
        High = Normal * 2,
    }
}
