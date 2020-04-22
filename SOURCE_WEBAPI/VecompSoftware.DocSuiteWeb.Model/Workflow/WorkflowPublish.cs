namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowPublish
    {
        #region [ Properties ]
        /// <summary>
        /// Propietà che contiene il file XAML.
        /// </summary>
        public string WorkflowXaml { get; set; }

        /// <summary>
        /// Specifica il nome del workflow.
        /// Il nome deve essere contenuto nella direttiva x:Class del <see cref="WorkflowXaml"/>.
        /// Il nome del workflow diventerà un nuovo Scope dell'ambiente workflow.
        /// </summary>
        public string WorkflowName { get; set; }
        #endregion

        #region [ Constructor ]
        public WorkflowPublish()
        { }
        #endregion
    }
}
