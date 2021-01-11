using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{

    public class ProtocolRoleUser : DSWBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]
        public ProtocolRoleUser() : this(Guid.NewGuid()) { }

        public ProtocolRoleUser(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public short Year { get; set; }
        public int Number { get; set; }
        public string GroupName { get; set; }
        public string UserName { get; set; }
        public string Account { get; set; }
        public byte IsActive { get; set; }
        public short Status { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual Protocol Protocol { get; set; }
        public virtual Role Role { get; set; }
        #endregion
    }
}
