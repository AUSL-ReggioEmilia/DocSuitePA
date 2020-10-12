using System;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities.Common
{
    public class DocSuiteEvent : BaseEntity
    {
        #region [ Properties ] 
        public int Year { get; set; }
        public string Number { get; set; }
        public string CategoryName { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset? WFERPStarted { get; set; }
        public WorkflowStatus? WFERPStatus { get; set; }
        #endregion
    }
}
