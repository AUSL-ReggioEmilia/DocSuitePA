using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Workflows
{
    public enum WorkflowStatus : short
    {
        Invalid = 0,
        Started = 1,
        Progress = 2 * Started,
        Done = 2 * Progress,
        Error = 2 * Done
    }
}
