using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class SecurityGroup : DSWBaseEntity
    {
        #region [ Constructor ]
        public SecurityGroup() : this(Guid.NewGuid()) { }

        public SecurityGroup(Guid uniqueId)
            : base(uniqueId)
        {
            SecurityUsers = new HashSet<SecurityUser>();
            ContainerGroups = new HashSet<ContainerGroup>();
            RoleGroups = new HashSet<RoleGroup>();
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Nome del gruppo
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Descrizione description
        /// </summary>
        public string LogDescription { get; set; }

        public bool IsAllUsers { get; set; }

        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Collezione degli utenti
        /// </summary>
        public virtual ICollection<SecurityUser> SecurityUsers { get; set; }

        public virtual ICollection<ContainerGroup> ContainerGroups { get; set; }

        public virtual ICollection<RoleGroup> RoleGroups { get; set; }

        #endregion
    }
}
