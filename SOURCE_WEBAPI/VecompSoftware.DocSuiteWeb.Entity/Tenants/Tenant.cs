using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Entity.Tenants
{

    public class Tenant : DSWTenantBaseEntity
    {
        #region [ Constructor ]

        public Tenant() : this(Guid.NewGuid()) { }

        public Tenant(Guid uniqueId)
            : base(uniqueId)
        {
            Configurations = new HashSet<TenantConfiguration>();
            Containers = new HashSet<Container>();
            Roles = new HashSet<Role>();
            PECMailBoxes = new HashSet<PECMailBox>();
            TenantWorkflowRepositories = new HashSet<TenantWorkflowRepository>();
            Contacts = new HashSet<Contact>();
        }

        #endregion

        #region [ Properties ]
        public string TenantName { get; set; }
        public string CompanyName { get; set; }

        #endregion

        #region[ Navigation Properties ]

        public virtual ICollection<TenantConfiguration> Configurations { get; set; }
        public virtual ICollection<Container> Containers { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<PECMailBox> PECMailBoxes { get; set; }
        public virtual ICollection<TenantWorkflowRepository> TenantWorkflowRepositories { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }

        #endregion
    }
}
