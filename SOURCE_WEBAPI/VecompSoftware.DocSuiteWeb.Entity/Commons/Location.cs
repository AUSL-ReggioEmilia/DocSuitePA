using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    /// <summary>
    /// La proprietà idLocation non ha identity perchè viene inserita manualmente
    /// </summary>
    public class Location : DSWBaseEntity
    {
        #region [ Constructor ]

        public Location() : this(Guid.NewGuid()) { }

        public Location(Guid uniqueId)
            : base(uniqueId)
        {
            DocmContainers = new HashSet<Container>();
            ProtContainers = new HashSet<Container>();
            ReslContainers = new HashSet<Container>();
            DeskContainers = new HashSet<Container>();
            UDSContainers = new HashSet<Container>();
            ProtAttachContainers = new HashSet<Container>();
            DocumentSeriesContainers = new HashSet<Container>();
            DocumentSeriesAnnexedContainers = new HashSet<Container>();
            DocumentSeriesUnpublishedAnnexedContainers = new HashSet<Container>();
            Messages = new HashSet<Message>();
            Protocols = new HashSet<Protocol>();
            PECMails = new HashSet<PECMail>();
            PECMailBoxes = new HashSet<PECMailBox>();
            Collaborations = new HashSet<Collaboration>();
            DocumentSeriesItems = new HashSet<DocumentSeriesItem>();
            DocumentSeriesItemAnnexes = new HashSet<DocumentSeriesItem>();
            DocumentSeriesItemUnpublishedAnnexes = new HashSet<DocumentSeriesItem>();
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Get or set name location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set Biblos protocol archive name
        /// </summary>
        public string ProtocolArchive { get; set; }

        /// <summary>
        /// Get or set Biblos document archive name
        /// </summary>
        public string DossierArchive { get; set; }

        /// <summary>
        /// Get or set Biblos resolution archive name
        /// </summary>
        public string ResolutionArchive { get; set; }

        /// <summary>
        /// Get or set Biblos conservation archive name
        /// </summary>
        public string ConservationArchive { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<Container> DocmContainers { get; set; }
        public virtual ICollection<Container> ProtContainers { get; set; }
        public virtual ICollection<Container> ReslContainers { get; set; }
        public virtual ICollection<Container> DeskContainers { get; set; }
        public virtual ICollection<Container> UDSContainers { get; set; }
        public virtual ICollection<Container> ProtAttachContainers { get; set; }
        public virtual ICollection<Container> DocumentSeriesContainers { get; set; }
        public virtual ICollection<Container> DocumentSeriesAnnexedContainers { get; set; }
        public virtual ICollection<Container> DocumentSeriesUnpublishedAnnexedContainers { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Protocol> Protocols { get; set; }
        public virtual ICollection<Protocol> AttachProtocols { get; set; }
        public virtual ICollection<PECMail> PECMails { get; set; }
        public virtual ICollection<PECMailBox> PECMailBoxes { get; set; }
        public virtual ICollection<Collaboration> Collaborations { get; set; }
        public virtual ICollection<DocumentSeriesItem> DocumentSeriesItems { get; set; }
        public virtual ICollection<DocumentSeriesItem> DocumentSeriesItemAnnexes { get; set; }
        public virtual ICollection<DocumentSeriesItem> DocumentSeriesItemUnpublishedAnnexes { get; set; }
        #endregion
    }
}
