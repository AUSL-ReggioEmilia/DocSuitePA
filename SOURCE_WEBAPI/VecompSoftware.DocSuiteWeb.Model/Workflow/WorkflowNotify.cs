using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowNotify
    {
        #region [ Constructor ]
        public WorkflowNotify()
        { }

        public WorkflowNotify(Guid workflowActivityId)
        {
            WorkflowActivityId = workflowActivityId;
            OutputArguments = new Dictionary<string, WorkflowArgument>();
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Nome dell'evento di notifica
        /// </summary>
        public string EventName => "DSW_WF_PUSH";

        /// <summary>
        /// Nome del modulo di provenzienza dell'evento
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Proprietà che contiene il nome del workflow
        /// </summary>
        public string WorkflowName { get; set; }

        /// <summary>
        /// Id della WorkflowActivity
        /// </summary>
        public Guid WorkflowActivityId { get; set; }
        /// <summary>
        /// Argomento di completamento dell'activity
        /// </summary>
        public Dictionary<string, WorkflowArgument> OutputArguments { get; set; }
        #endregion
    }
}
