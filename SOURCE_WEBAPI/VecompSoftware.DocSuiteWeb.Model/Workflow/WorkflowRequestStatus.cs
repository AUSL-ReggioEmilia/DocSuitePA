using System;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowRequestStatus
    {
        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid InstanceId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public string WorkflowName { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string StackTrace { get; set; }
        public DateTimeOffset LogDate { get; set; }

        #endregion
    }
}
