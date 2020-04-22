using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowStep
    {
        public WorkflowStep()
        {
            InputArguments = new HashSet<WorkflowArgument>();
            OutputArguments = new HashSet<WorkflowArgument>();
            EvaluationArguments = new HashSet<WorkflowArgument>();
            StepId = Guid.NewGuid();
        }

        public int Position { get; set; }

        public Guid StepId { get; set; }

        /// <summary>
        /// sintassi c# che deve generare una List<Guid> che corrispondono agli StepId 
        /// looking next steps using Guid instead of actual algorithm CurrentPosition+1). This pattern can be usefull for If statement or while/do->while
        /// Questa stringa va dato inpasto al valutatore della semantica dinamica di Roslyn. 
        /// ES DynamiCondition -> TRUE -> List({DBFEF586-E97D-4E23-9C04-AEBD782A2069})
        /// ES DynamiCondition -> c# expression defined in IF statement -> If condition is true -> List({2243E290-77E5-423E-828A-BCDA22F5C4FF}) else List({C2FFD42B-B0D7-411F-A251-0C35A4A986CD})
        /// </summary>
        public string EvaluationPosition { get; set; }

        public string Name { get; set; }

        public WorkflowAuthorizationType AuthorizationType { get; set; }

        public WorkflowActivityOperation ActivityOperation { get; set; }

        public WorkflowActivityType ActivityType { get; set; }

        public ICollection<WorkflowArgument> EvaluationArguments { get; set; }

        public ICollection<WorkflowArgument> InputArguments { get; set; }

        public ICollection<WorkflowArgument> OutputArguments { get; set; }
    }
}
