using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Tasks
{
    public class TaskDetail : DSWBaseEntity
    {
        #region [ Constructor ]
        public TaskDetail() : this(Guid.NewGuid()) { }

        public TaskDetail(Guid uniqueId) : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]
        public TaskDetailType DetailType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ErrorDescription { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual TaskHeader TaskHeader { get; set; }
        #endregion
    }
}
