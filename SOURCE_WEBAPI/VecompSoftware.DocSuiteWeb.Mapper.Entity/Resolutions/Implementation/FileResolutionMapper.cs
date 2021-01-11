using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class FileResolutionMapper : BaseEntityMapper<FileResolution, FileResolution>, IFileResolutionMapper
    {
        public override FileResolution Map(FileResolution entity, FileResolution entityTransformed)
        {
            #region [ Base ]
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

            return entityTransformed;
        }
    }
}
