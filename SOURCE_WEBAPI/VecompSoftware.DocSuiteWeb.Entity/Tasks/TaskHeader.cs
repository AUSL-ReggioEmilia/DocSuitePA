using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.Tasks
{
    public class TaskHeader : DSWBaseEntity
    {
        #region [ Constructor ]
        public TaskHeader() : this(Guid.NewGuid()) { }

        public TaskHeader(Guid uniqueId) : base(uniqueId)
        {
            TaskHeaderProtocols = new HashSet<TaskHeaderProtocol>();
        }
        #endregion

        #region [ Properties ]
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskType TaskType { get; set; }
        public TaskStatus Status { get; set; }
        public TaskHeaderSendingProcessStatus? SendingProcessStatus { get; set; }
        public TaskHeaderSendedStatus? SendedStatus { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<TaskHeaderProtocol> TaskHeaderProtocols { get; set; }
        #endregion
    }
}
