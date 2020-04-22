using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public enum DeskState
    {
        /// <summary>
        /// Tavolo creato e aperto
        /// </summary>
        [Description("aperto")]
        Open = 1,
        /// <summary>
        /// Tavolo chiuso
        /// </summary>
        [Description("chiuso")]
        Closed = 2,
        /// <summary>
        /// Tavolo in Approvazione
        /// </summary>
        [Description("in approvazione")]
        Approve = 2 * Closed
    }
}
