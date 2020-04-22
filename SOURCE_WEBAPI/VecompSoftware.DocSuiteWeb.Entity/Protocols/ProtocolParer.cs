using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{

    public class ProtocolParer : DSWBaseEntity
    {
        #region [ Constructor ]
        public ProtocolParer() : this(Guid.NewGuid()) { }

        public ProtocolParer(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public short Year { get; set; }
        public int Number { get; set; }

        public DateTime? ArchiviedDate { get; set; }
        public string ParerUri { get; set; }
        public bool? HasError { get; set; }
        public short? IsForArchive { get; set; }
        public string LastError { get; set; }
        public DateTime? LastSendDate { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual Protocol Protocol { get; set; }
        #endregion
    }
}
