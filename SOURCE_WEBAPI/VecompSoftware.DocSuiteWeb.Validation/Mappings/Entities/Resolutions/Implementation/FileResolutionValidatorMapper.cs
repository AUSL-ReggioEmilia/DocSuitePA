using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions
{
    public class FileResolutionValidatorMapper : BaseMapper<FileResolution, FileResolutionValidator>, IFileResolutionValidatorMapper
    {
        public FileResolutionValidatorMapper() { }

        public override FileResolutionValidator Map(FileResolution entity, FileResolutionValidator entityTransformed)
        {

            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.IdResolutionFile = entity.IdResolutionFile;
            entityTransformed.IdProposalFile = entity.IdProposalFile;
            entityTransformed.IdAttachments = entity.IdAttachments;
            entityTransformed.IdControllerFile = entity.IdControllerFile;
            entityTransformed.IdAssumedProposal = entity.IdAssumedProposal;
            entityTransformed.IdFrontespizio = entity.IdFrontespizio;
            entityTransformed.IdPrivacyAttachments = entity.IdPrivacyAttachments;
            entityTransformed.IdFrontalinoRitiro = entity.IdFrontalinoRitiro;
            entityTransformed.IdAnnexes = entity.IdAnnexes;
            entityTransformed.IdPrivacyPublicationDocument = entity.IdPrivacyPublicationDocument;
            entityTransformed.IdMainDocumentsOmissis = entity.IdMainDocumentsOmissis;
            entityTransformed.IdAttachmentsOmissis = entity.IdAttachmentsOmissis;
            entityTransformed.IdUltimaPaginaFile = entity.IdUltimaPaginaFile;
            entityTransformed.IdSupervisoryBoardFile = entity.IdSupervisoryBoardFile;
            entityTransformed.DematerialisationChainId = entity.DematerialisationChainId;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Resolution = entity.Resolution;
            #endregion

            return entityTransformed;
        }

    }
}
