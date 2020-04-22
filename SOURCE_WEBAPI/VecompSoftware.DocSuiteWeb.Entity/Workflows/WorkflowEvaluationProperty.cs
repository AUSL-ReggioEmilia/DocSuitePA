using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{
    public class WorkflowEvaluationProperty : DSWBaseEntity
    {
        #region [ Constructor ]
        public WorkflowEvaluationProperty() : this(Guid.NewGuid()) { }

        public WorkflowEvaluationProperty(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public WorkflowType WorkflowType { get; set; }
        public WorkflowPropertyType PropertyType { get; set; }
        public long? ValueInt { get; set; }
        public DateTime? ValueDate { get; set; }
        public double? ValueDouble { get; set; }
        public bool? ValueBoolean { get; set; }
        public Guid? ValueGuid { get; set; }
        public string ValueString { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual WorkflowRepository WorkflowRepository { get; set; }
        #endregion
    }
}
