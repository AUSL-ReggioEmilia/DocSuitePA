
using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{
    public enum WorkflowStatus : short
    {
        /// <summary>
        /// Attiva
        /// </summary>
        [Description("Da lavorare")]
        Todo = 1,

        /// <summary>
        /// In esecuzione
        /// </summary>
        [Description("In corso")]
        Progress = 2,

        /// <summary>
        /// Sospesa
        /// </summary>
        [Description("Sospesa")]
        Suspended = 2 * Progress,

        /// <summary>
        /// Eseguita
        /// </summary>
        [Description("Completata")]
        Done = 2 * Suspended,

        /// <summary>
        /// In errore
        /// </summary>
        [Description("In errore")]
        Error = 2 * Done,

        /// <summary>
        /// Annullamento logico
        /// </summary>
        [Description("Annullamento")]
        LogicalDelete = 2 * Error
    }
}
