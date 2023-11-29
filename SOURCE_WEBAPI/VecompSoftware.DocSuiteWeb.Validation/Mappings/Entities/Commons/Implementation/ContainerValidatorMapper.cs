using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class ContainerValidatorMapper : BaseMapper<Container, ContainerValidator>, IContainerValidatorMapper
    {
        public ContainerValidatorMapper() { }

        public override ContainerValidator Map(Container entity, ContainerValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityShortId = entity.EntityShortId;
            entityTransformed.Name = entity.Name;
            entityTransformed.isActive = entity.isActive;
            entityTransformed.idArchive = entity.idArchive;
            entityTransformed.Note = entity.Note;
            entityTransformed.Privacy = entity.Privacy;
            entityTransformed.HeadingFrontalino = entity.HeadingFrontalino;
            entityTransformed.HeadingLetter = entity.HeadingLetter;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.AutomaticSecurityGroups = entity.AutomaticSecurityGroups;
            entityTransformed.PrefixSecurityGroupName = entity.PrefixSecurityGroupName;
            entityTransformed.ContainerType = entity.ContainerType;
            entityTransformed.SecurityUserAccount = entity.SecurityUserAccount;
            entityTransformed.SecurityUserDisplayName = entity.SecurityUserDisplayName;
            entityTransformed.PrivacyLevel = entity.PrivacyLevel;
            entityTransformed.PrivacyEnabled = entity.PrivacyEnabled;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocmLocation = entity.DocmLocation;
            entityTransformed.ReslLocation = entity.ReslLocation;
            entityTransformed.ProtLocation = entity.ProtLocation;
            entityTransformed.Protocols = entity.Protocols;
            entityTransformed.Resolutions = entity.Resolutions;
            entityTransformed.OChartItemContainers = entity.OChartItemContainers;
            entityTransformed.ContainerGroups = entity.ContainerGroups;
            entityTransformed.DeskLocation = entity.DeskLocation;
            entityTransformed.UDSLocation = entity.UDSLocation;
            entityTransformed.DocumentSeriesAnnexedLocation = entity.DocumentSeriesAnnexedLocation;
            entityTransformed.DocumentSeriesLocation = entity.DocumentSeriesLocation;
            entityTransformed.DocumentSeriesUnpublishedAnnexedLocation = entity.DocumentSeriesUnpublishedAnnexedLocation;
            entityTransformed.ProtAttachLocation = entity.ProtAttachLocation;
            entityTransformed.DocumentUnits = entity.DocumentUnits;
            entityTransformed.DocumentSeries = entity.DocumentSeries;
            entityTransformed.UDSRepositories = entity.UDSRepositories;
            entityTransformed.Dossiers = entity.Dossiers;
            #endregion

            return entityTransformed;
        }

    }
}
