﻿using System;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Entity.Parameters
{
    public class Parameter : DSWBaseEntity
    {
        #region [ Constructor ]
        public Parameter() : this(Guid.NewGuid()) { }
        public Parameter(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region [ Properties ]

        public int Incremental { get; set; }
        public short LastUsedYear { get; set; }
        public int LastUsedNumber { get; set; }
        public bool Locked { get; set; }
        public short LastUsedIdCategory { get; set; }
        public short LastUsedIdContainer { get; set; }
        public short LastUsedIdRole { get; set; }
        public short LastUsedIdRoleUser { get; set; }
        public int? LastUsedIdResolution { get; set; }
        public short LastUsedResolutionYear { get; set; }
        public short LastUsedResolutionNumber { get; set; }
        public short LastUsedBillNumber { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual TenantAOO TenantAOO { get; set; }
        #endregion
    }
}
