using System;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public class ProtocolUser : DSWBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]

        public ProtocolUser() : this(Guid.NewGuid()) { }
        public ProtocolUser(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region[ Properties ]
        public short Year { get; set; }
        public int Number { get; set; }
        public string Account { get; set; }
        public ProtocolUserType Type { get; set; }
        public string Note { get; set; }
        #endregion

        #region[ Navigation Properties ]

        public virtual Protocol Protocol { get; set; }

        #endregion
    }
}
