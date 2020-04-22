using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowResult
    {
        #region [ Properties ]
        public bool IsValid { get; set; }
        public ICollection<TranslationError> Errors { get; set; }
        public Guid? InstanceId { get; set; }
        #endregion

        #region [ Constructor ]
        public WorkflowResult()
        {
            Errors = new Collection<TranslationError>();
        }
        #endregion
    }
}
