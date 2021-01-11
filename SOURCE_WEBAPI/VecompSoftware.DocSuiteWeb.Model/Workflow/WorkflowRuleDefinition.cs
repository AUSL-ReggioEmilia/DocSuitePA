using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowRuleDefinition
    {
        #region [ Constructor ]
        public WorkflowRuleDefinition()
        {
            Rules = new List<WorkflowRule>();
        }
        #endregion

        #region [ Properties ]
        public DSWEnvironmentType Environment { get; set; }

        public ICollection<WorkflowRule> Rules { get; set; }
        #endregion


    }
}
