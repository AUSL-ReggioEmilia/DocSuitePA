using System;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR.Common;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR
{
    public class SkyDocEvent : BaseEntity
    {
        #region [ Properties ] 
        public SkyDocEventType EventType { get; set; }
        public int Year { get; set; }
        public string Number { get; set; }
        public string CategoryName { get; set; }
        public string Subject { get; set; }
        public Guid EntityUniqueId { get; set; } 
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset? WFEHCStarted { get; set; }
        public WorkflowStatus? WFEHCStatus { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual SkyDocCommand Command { get; set; }
        #endregion
    }
}
