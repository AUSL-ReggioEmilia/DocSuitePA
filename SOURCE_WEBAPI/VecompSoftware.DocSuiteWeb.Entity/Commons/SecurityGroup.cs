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
            GroupChildren = new HashSet<SecurityGroup>();
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
        /// Path incrementale del gruppo di sicurezza
        /// </summary>
        public string FullIncrementalPath { get; set; }

        /// <summary>
        /// Descrizione description
        /// </summary>
        public string LogDescription { get; set; }

        public bool IsAllUsers { get; set; }

        public int IdSecurityGroupTenant { get; set; }

        public Guid TenantId { get; set; }
        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Gruppo padre
        /// </summary>
        public SecurityGroup GroupFather { get; set; }
        /// <summary>
        /// Gruppo padre
        /// </summary>
        public virtual ICollection<SecurityGroup> GroupChildren { get; set; }
        /// <summary>
        /// Collezione degli utenti
        /// </summary>
        public virtual ICollection<SecurityUser> SecurityUsers { get; set; }

        public virtual ICollection<ContainerGroup> ContainerGroups { get; set; }

        public virtual ICollection<RoleGroup> RoleGroups { get; set; }

        #endregion
    }
}
