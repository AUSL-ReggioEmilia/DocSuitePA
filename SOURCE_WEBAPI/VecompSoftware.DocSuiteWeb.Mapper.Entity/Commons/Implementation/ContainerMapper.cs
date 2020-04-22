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
            entityTransformed.Massive = entity.Massive;
            entityTransformed.Note = entity.Note;
            entityTransformed.Privacy = entity.Privacy;
            entityTransformed.ActiveFrom = entity.ActiveFrom;
            entityTransformed.ActiveTo = entity.ActiveTo;
            entityTransformed.Conservation = entity.Conservation;
            entityTransformed.DocumentSeriesAnnexedLocation = entity.DocumentSeriesAnnexedLocation;
            entityTransformed.DocumentSeriesLocation = entity.DocumentSeriesLocation;
            entityTransformed.DocumentSeriesUnpublishedAnnexedLocation = entity.DocumentSeriesUnpublishedAnnexedLocation;
            entityTransformed.ProtocolRejection = entity.ProtocolRejection;
            entityTransformed.HeadingFrontalino = entity.HeadingFrontalino;
            entityTransformed.HeadingLetter = entity.HeadingLetter;
            entityTransformed.ProtAttachLocation = entity.ProtAttachLocation;
            entityTransformed.ManageSecureDocument = entity.ManageSecureDocument;
            entityTransformed.PrivacyLevel = entity.PrivacyLevel;
            entityTransformed.PrivacyEnabled = entity.PrivacyEnabled;
            #endregion

            return entityTransformed;
        }

    }
}
