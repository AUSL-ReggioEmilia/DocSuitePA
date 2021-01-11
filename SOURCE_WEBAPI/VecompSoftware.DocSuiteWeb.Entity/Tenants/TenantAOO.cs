
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Entity.Tenants
{
    public class TenantAOO : DSWBaseEntity
    {
        #region [ Constructor ]
        public TenantAOO() : this(Guid.NewGuid()) { }

        public TenantAOO(Guid uniqueId) : base(uniqueId)
        {
            Tenants = new HashSet<Tenant>();
            DocumentUnits = new HashSet<DocumentUnit>();
            Parameters = new HashSet<Parameter>();
            Protocols = new HashSet<Protocol>();
            Categories = new HashSet<Category>();
        }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public string Note { get; set; }
        public string CategorySuffix { get; set; }
        public TenantTypologyType TenantTypology { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<Tenant> Tenants { get; set; }
        public virtual ICollection<DocumentUnit> DocumentUnits { get; set; }
        public virtual ICollection<Parameter> Parameters { get; set; }
        public virtual ICollection<Protocol> Protocols { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        #endregion
    }
}
