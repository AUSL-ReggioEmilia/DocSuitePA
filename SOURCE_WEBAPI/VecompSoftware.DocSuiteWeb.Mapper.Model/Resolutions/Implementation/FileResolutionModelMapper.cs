using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Resolutions
{
    public class FileResolutionModelMapper : BaseModelMapper<FileResolution, FileResolutionModel>, IFileResolutionModelMapper
    {
        public override FileResolutionModel Map(FileResolution entity, FileResolutionModel entityTransformed)
        {
            entityTransformed.IdResolution = entity.EntityId;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IdProposalFile = entity.IdProposalFile;
            entityTransformed.IdAttachments = entity.IdAttachments;
            entityTransformed.IdControllerFile = entity.IdControllerFile;
            entityTransformed.IdAssumedProposal = entity.IdAssumedProposal;
            entityTransformed.IdFrontalinoRitiro = entity.IdFrontalinoRitiro;
            entityTransformed.IdFrontespizio = entity.IdFrontespizio;
            entityTransformed.IdPrivacyAttachments = entity.IdPrivacyAttachments;
            entityTransformed.IdAnnexes = entity.IdAnnexes;
            entityTransformed.IdPrivacyPublicationDocument = entity.IdPrivacyPublicationDocument;
            entityTransformed.IdMainDocumentsOmissis = entity.IdMainDocumentsOmissis;
            entityTransformed.IdAttachmentsOmissis = entity.IdAttachmentsOmissis;
            entityTransformed.IdUltimaPaginaFile = entity.IdUltimaPaginaFile;
            entityTransformed.IdSupervisoryBoardFile = entity.IdSupervisoryBoardFile;
            entityTransformed.DematerialisationChainId = entity.DematerialisationChainId;

            return entityTransformed;
        }
    }
}
