using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public class ProtocolStatus : DSWBaseEntity
    {
        #region [ Constructor ]

        public ProtocolStatus() : this(Guid.NewGuid()) { }
        public ProtocolStatus(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region[ Properties ]
        public string Status { get; set; }
        public string ProtocolStatusDescription { get; set; }
        public string BackColor { get; set; }
        public string ForeColor { get; set; }

        #endregion

        #region[ Navigation Properties ]
        #endregion
    }
}
