using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class Container : DSWBaseEntity
    {
        #region [ Constructor ]

        public Container() : this(Guid.NewGuid()) { }
        public Container(Guid uniqueId)
            : base(uniqueId)
        {
            Protocols = new HashSet<Protocol>();
            Resolutions = new HashSet<Resolution>();
            OChartItemContainers = new HashSet<OChartItemContainer>();
            ContainerGroups = new HashSet<ContainerGroup>();
            DocumentSeries = new HashSet<DocumentSeries>();
            DocumentUnits = new HashSet<DocumentUnit>();
            UDSRepositories = new HashSet<UDSRepository>();
            Dossiers = new HashSet<Dossier>();
            ContainerProperties = new HashSet<ContainerProperty>();
            Tenants = new HashSet<Tenant>();
            Fascicles = new HashSet<Fascicle>();
        }
        #endregion

        #region[ Properties ]

        public string Name { get; set; }
        public string Note { get; set; }
        public bool isActive { get; set; }
        public int? idArchive { get; set; }
        public byte? Privacy { get; set; }
        public string HeadingFrontalino { get; set; }
        public string HeadingLetter { get; set; }
        public int PrivacyLevel { get; set; }
        public bool PrivacyEnabled { get; set; }

        #region [fake properties]
        /// <summary>
        /// Proprietà fake per gestire l'inserimento automatico di gruppi
        /// </summary>
        public bool? AutomaticSecurityGroups { get; set; }
        /// <summary>
        /// Proprietà fake per gestire l'inserimento automatico di gruppi
        /// </summary>
        public string PrefixSecurityGroupName { get; set; }
        /// <summary>
        /// Proprietà fake per gestire l'inserimento automatico di gruppi
        /// </summary>
        public ContainerType? ContainerType { get; set; }
        /// <summary>
        /// Proprietà fake per gestire l'inserimento automatico in SecurityUser
        /// </summary>
        public string SecurityUserAccount { get; set; }
        /// <summary>
        /// Proprietà fake per gestire l'inserimento automatico in SecurityUser
        /// </summary>
        public string SecurityUserDisplayName { get; set; }
        #endregion

        #endregion

        #region [ Navigation Properties ]
        public virtual Location DocmLocation { get; set; }
        public virtual Location ProtLocation { get; set; }
        public virtual Location ReslLocation { get; set; }
        public virtual Location DeskLocation { get; set; }
        public virtual Location UDSLocation { get; set; }
        public virtual Location ProtAttachLocation { get; set; }
        public virtual Location DocumentSeriesLocation { get; set; }
        public virtual Location DocumentSeriesUnpublishedAnnexedLocation { get; set; }
        public virtual Location DocumentSeriesAnnexedLocation { get; set; }
        public virtual ICollection<Protocol> Protocols { get; set; }
        public virtual ICollection<Resolution> Resolutions { get; set; }
        public virtual ICollection<OChartItemContainer> OChartItemContainers { get; set; }
        public virtual ICollection<ContainerGroup> ContainerGroups { get; set; }
        public virtual ICollection<DocumentSeries> DocumentSeries { get; set; }
        public virtual ICollection<DocumentUnit> DocumentUnits { get; set; }
        public virtual ICollection<UDSRepository> UDSRepositories { get; set; }
        public virtual ICollection<Dossier> Dossiers { get; set; }
        public virtual ICollection<ContainerProperty> ContainerProperties { get; set; }
        public virtual ICollection<Tenant> Tenants { get; set; }
        public virtual ICollection<Fascicle> Fascicles { get; set; }
        public virtual ICollection<CategoryFascicleRight> CategoryFascicleRights { get; set; }
        #endregion
    }
}
