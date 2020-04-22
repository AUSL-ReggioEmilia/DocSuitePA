using System;

namespace VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts
{
    public class JeepServiceHost : DSWBaseEntity
    {
        #region [ Constructor ]
        public JeepServiceHost() : this(Guid.NewGuid()) { }

        public JeepServiceHost(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public string Hostname { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        #endregion
    }
}
