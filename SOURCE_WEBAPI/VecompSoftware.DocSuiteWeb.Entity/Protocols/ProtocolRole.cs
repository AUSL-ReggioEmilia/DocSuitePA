using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{

    public class ProtocolRole : DSWBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]
        public ProtocolRole() : this(Guid.NewGuid()) { }

        public ProtocolRole(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public short Year { get; set; }
        public int Number { get; set; }
        public string Rights { get; set; }
        public string Note { get; set; }
        public string Type { get; set; }
        public string DistributionType { get; set; }
        public ProtocolRoleNoteType? NoteType { get; set; }
        public ProtocolRoleStatus Status { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual Role Role { get; set; }
        public virtual Protocol Protocol { get; set; }
        #endregion
    }
}
