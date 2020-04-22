using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
{
    public enum WorkflowStatus : short
    {
        /// <summary>
        /// Activity Attiva
        /// </summary>
        [Description("Da lavorare")]
        Active = 1,

        /// <summary>
        /// Activity In Esecuzione
        /// </summary>
        [Description("In corso")]
        Progress = 2,

        /// <summary>
        /// Activity Sospesa
        /// </summary>
        [Description("Sospesa")]
        Suspended = 2 * Progress,

        /// <summary>
        /// Activity Eseguita
        /// </summary>
        [Description("Completata")]
        Done = 2 * Suspended,

        /// <summary>
        /// Activity In Errore
        /// </summary>
        [Description("In errore")]
        Error = 2 * Done,

        /// <summary>
        /// Activity In Errore
        /// </summary>
        [Description("Annullamento")]
        LogicalDelete = 2 * Error
    }
}
