using System;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Entity.Tasks
{
    public class TaskHeaderProtocol : DSWBaseEntity
    {
        #region [ Constructor ]
        public TaskHeaderProtocol() : this(Guid.NewGuid()) { }

        public TaskHeaderProtocol(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]
        public short? Year { get; set; }
        public int? Number { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual TaskHeader TaskHeader { get; set; }
        public virtual Protocol Protocol { get; set; }
        #endregion
    }
}
