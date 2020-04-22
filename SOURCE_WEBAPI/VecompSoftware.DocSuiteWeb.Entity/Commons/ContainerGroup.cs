using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class ContainerGroup : DSWBaseEntity
    {
        #region [ Constructor ]

        public ContainerGroup() : this(Guid.NewGuid()) { }
        public ContainerGroup(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region[ Properties ]

        public string GroupName { get; set; }
        public string ProtocolRights { get; set; }
        public string ResolutionRights { get; set; }
        public string DocumentRights { get; set; }
        public string DocumentSeriesRights { get; set; }
        public string DeskRights { get; set; }
        public string UDSRights { get; set; }
        public int PrivacyLevel { get; set; }
        public string FascicleRights { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual Container Container { get; set; }

        public virtual SecurityGroup SecurityGroup { get; set; }


        #endregion
    }
}
