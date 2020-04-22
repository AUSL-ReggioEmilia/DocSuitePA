using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class LocationValidator : ObjectValidator<Location, LocationValidator>, ILocationValidator
    {
        #region [ Constructor ]
        public LocationValidator(ILogger logger, ILocationValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region [ Properties ]
        public short EntityShortId { get; set; }

        /// <summary>
        /// Get or set name location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set Biblos server name
        /// </summary>
        public string DocumentServer { get; set; }

        /// <summary>
        /// Get or set Biblos protocol archive name
        /// </summary>
        public string ProtBiblosDsDb { get; set; }

        /// <summary>
        /// Get or set Biblos document archive name
        /// </summary>
        public string DocmBiblosDsDb { get; set; }

        /// <summary>
        /// Get or set Biblos resolution archive name
        /// </summary>
        public string ReslBiblosDsDb { get; set; }

        /// <summary>
        /// Get or set Biblos conservation archive name
        /// </summary>
        public string ConsBiblosDsDb { get; set; }

        /// <summary>
        /// Get or set Biblos conservation server name
        /// </summary>
        public string ConservationServer { get; set; }
        public Guid UniqueId { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Location DocmLocation { get; set; }
        public Location ProtLocation { get; set; }
        public Location ReslLocation { get; set; }
        public ICollection<Container> DocmContainers { get; set; }
        public ICollection<Container> ProtContainers { get; set; }
        public ICollection<Container> ReslContainers { get; set; }
        public ICollection<Container> DeskContainers { get; set; }
        public ICollection<Container> UDSContainers { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Protocol> Protocols { get; set; }
        public ICollection<PECMail> PECMails { get; set; }
        public ICollection<PECMailBox> PECMailBoxes { get; set; }
        public ICollection<Collaboration> Collaborations { get; set; }
        public ICollection<DocumentSeriesItem> DocumentSeriesItems { get; set; }
        public ICollection<DocumentSeriesItem> DocumentSeriesItemAnnexes { get; set; }
        public ICollection<DocumentSeriesItem> DocumentSeriesItemUnpublishedAnnexes { get; set; }

        #endregion
    }
}
