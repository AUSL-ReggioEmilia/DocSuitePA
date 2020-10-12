using System;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities.Common
{
    public class DocSuiteDocument : BaseEntity
    {
        #region [ Properties ]
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? WFDocSuiteId { get; set; }
        public DateTimeOffset? WFDocSuiteStarted { get; set; }
        public WorkflowStatus? WFDocSuiteStatus { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual DocSuiteCommand Command { get; set; }
        #endregion
    }
}
