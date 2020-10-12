using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities.Common
{
    public class DocSuiteCommand : BaseEntity
    {
        public DocSuiteCommand()
        {
            Documents = new HashSet<DocSuiteDocument>();
        }

        #region [ Properties ]
        public string SupplierName { get; set; }
        public string SupplierPIVACF { get; set; }
        public string Number { get; set; }
        public string CostCenter { get; set; }
        public Guid TenantId { get; set; }
        public Guid? WFDocSuiteId { get; set; }
        public Guid? UDSId { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public DateTimeOffset? WFDocSuiteStarted { get; set; }
        public WorkflowStatus? WFDocSuiteStatus { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<DocSuiteDocument> Documents { get; set; }
        #endregion
    }
}
