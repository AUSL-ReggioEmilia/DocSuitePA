using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowComplete
    {
        public WorkflowComplete()
        {
            OutputArguments = new Dictionary<string, WorkflowArgument>();
        }

        public Guid InstanceId { get; set; }

        public bool HasError { get; set; }

        public Dictionary<string, WorkflowArgument> OutputArguments { get; set; }
    }
}
