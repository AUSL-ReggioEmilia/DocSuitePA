using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class ContainerValidator : ObjectValidator<Container, ContainerValidator>, IContainerValidator
    {
        #region [ Constructor ]
        public ContainerValidator(ILogger logger, IContainerValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region[ Properties ]

        public short EntityShortId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public bool isActive { get; set; }
        public int? idArchive { get; set; }
        public byte? Privacy { get; set; }
        public string HeadingFrontalino { get; set; }
        public string HeadingLetter { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public Guid UniqueId { get; set; }
        public bool? AutomaticSecurityGroups { get; set; }
        public string PrefixSecurityGroupName { get; set; }
        public string SecurityUserAccount { get; set; }
        public string SecurityUserDisplayName { get; set; }
        public ContainerType? ContainerType { get; set; }
        public int PrivacyLevel { get; set; }
        public bool PrivacyEnabled { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Location DocmLocation { get; set; }
        public Location ProtLocation { get; set; }
        public Location ReslLocation { get; set; }
        public Location DeskLocation { get; set; }
        public Location UDSLocation { get; set; }
        public Location DocumentSeriesAnnexedLocation { get; set; }
        public Location DocumentSeriesLocation { get; set; }
        public Location DocumentSeriesUnpublishedAnnexedLocation { get; set; }
        public Location ProtAttachLocation { get; set; }
        public ICollection<Protocol> Protocols { get; set; }
        public ICollection<Resolution> Resolutions { get; set; }
        public ICollection<DocumentSeries> DocumentSeries { get; set; }
        public ICollection<OChartItemContainer> OChartItemContainers { get; set; }
        public ICollection<ContainerGroup> ContainerGroups { get; set; }
        public ICollection<DocumentUnit> DocumentUnits { get; set; }
        public ICollection<UDSRepository> UDSRepositories { get; set; }
        public ICollection<Dossier> Dossiers { get; set; }
        #endregion
    }
}