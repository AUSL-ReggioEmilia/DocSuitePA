namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowActivityOperation
    {
        #region [ Properties ]
        /// <summary>
        /// Proprietà che contiene la action del workflow
        /// </summary>
        public WorkflowActivityAction Action { get; set; }
        /// <summary>
        /// Proprietà che contiene le aree del workflow
        /// </summary>
        public WorkflowActivityArea Area { get; set; }
        #endregion

        #region [ Constructor ]
        public WorkflowActivityOperation()
        {
        }
        #endregion
    }

}
