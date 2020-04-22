using System;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowArgument
    {
        #region [ Properties ]

        public string Name { get; set; }

        public ArgumentType PropertyType { get; set; }

        public long? ValueInt { get; set; }

        public DateTime? ValueDate { get; set; }

        public double? ValueDouble { get; set; }

        public bool? ValueBoolean { get; set; }

        public Guid? ValueGuid { get; set; }

        public string ValueString { get; set; }

        #endregion
    }
}
