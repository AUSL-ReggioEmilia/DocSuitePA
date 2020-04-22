using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{

    public class ProtocolLink : DSWBaseEntity
    {
        #region [ Constructor ]
        public ProtocolLink() : this(Guid.NewGuid()) { }

        public ProtocolLink(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public short Year { get; set; }
        public int Number { get; set; }
        public bool? LinkType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Protocol Protocol { get; set; }

        public virtual Protocol ProtocolLinked { get; set; }

        #endregion
    }
}
