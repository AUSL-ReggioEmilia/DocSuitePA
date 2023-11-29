using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public class ProtocolJournal : DSWBaseEntity, IWorkflowContentBase
    {
        #region [ Constructor ]
        public ProtocolJournal() : this(Guid.NewGuid()) { }

        public ProtocolJournal(Guid uniqueId)
            : base(uniqueId)
        {
            Protocols = new HashSet<Protocol>();
        }
        #endregion

        #region [ Properties ]
        public DateTime ProtocolJournalDate { get; set; }
        public DateTime LogDate { get; set; }
        public string SystemComputer { get; set; }
        public string SystemUser { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ProtocolTotal { get; set; }
        public int? ProtocolRegister { get; set; }
        public int? ProtocolError { get; set; }
        public int? ProtocolCancelled { get; set; }
        public int ProtocolActive { get; set; }
        public int? ProtocolOthers { get; set; }
        public int? IdDocument { get; set; }
        public string LogDescription { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual TenantAOO TenantAOO { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Protocol> Protocols { get; set; }

        #endregion

        #region [ Not Mapping Properties ]
        public bool WorkflowAutoComplete { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public string WorkflowName { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }
        #endregion
    }
}
