using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowStart
    {
        #region [ Properties ]
        /// <summary>
        /// Proprietà che contiene il nome del workflow
        /// </summary>
        public string WorkflowName { get; set; }
        /// <summary>
        /// Argomenti in INGRESSO al workflow.
        /// </summary>
        public IDictionary<string, WorkflowArgument> Arguments { get; set; }
        /// <summary>
        /// Parametri di avvio del workflow.
        /// </summary>
        public IDictionary<string, object> StartParameters { get; set; }
        #endregion

        #region [ Constructor ]
        public WorkflowStart()
        {
            Arguments = new Dictionary<string, WorkflowArgument>();
            StartParameters = new Dictionary<string, object>();
        }
        #endregion
    }
}
