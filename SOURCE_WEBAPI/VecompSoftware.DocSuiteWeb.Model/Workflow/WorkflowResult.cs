using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowResult
    {
        #region [ Properties ]
        public bool IsValid { get; set; }
        public ICollection<string> Errors { get; set; }
        public Guid? InstanceId { get; set; }
        #endregion

        #region [ Constructor ]
        public WorkflowResult()
        {
            Errors = new List<string>();
        }
        #endregion
    }
}
