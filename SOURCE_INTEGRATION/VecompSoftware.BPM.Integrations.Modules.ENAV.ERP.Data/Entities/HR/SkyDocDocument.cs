using System;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR.Common;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR
{
    public class SkyDocDocument : BaseEntity
    {
        #region [ Properties ]
        public SkyDocDocumentType DocumentType { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? WFSkyDocId { get; set; }
        public DateTimeOffset? WFSkyDocStarted { get; set; }
        public WorkflowStatus? WFSkyDocStatus { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual SkyDocCommand Command { get; set; }
        #endregion
    }
}
