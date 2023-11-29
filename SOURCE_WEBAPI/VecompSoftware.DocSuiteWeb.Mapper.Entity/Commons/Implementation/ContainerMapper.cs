using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class ContainerMapper : BaseEntityMapper<Container, Container>, IContainerMapper
    {
        public ContainerMapper()
        {

        }

        public override Container Map(Container entity, Container entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.isActive = entity.isActive;
            entityTransformed.idArchive = entity.idArchive;
            entityTransformed.Note = entity.Note;
            entityTransformed.Privacy = entity.Privacy;
            entityTransformed.DocumentSeriesAnnexedLocation = entity.DocumentSeriesAnnexedLocation;
            entityTransformed.DocumentSeriesLocation = entity.DocumentSeriesLocation;
            entityTransformed.DocumentSeriesUnpublishedAnnexedLocation = entity.DocumentSeriesUnpublishedAnnexedLocation;
            entityTransformed.HeadingFrontalino = entity.HeadingFrontalino;
            entityTransformed.HeadingLetter = entity.HeadingLetter;
            entityTransformed.ProtAttachLocation = entity.ProtAttachLocation;
            entityTransformed.PrivacyLevel = entity.PrivacyLevel;
            entityTransformed.PrivacyEnabled = entity.PrivacyEnabled;
            #endregion

            return entityTransformed;
        }

    }
}
