using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions
{
    public class FileResolutionModel
    {
        #region [ Constructor ]
        public FileResolutionModel()
        {

        }

        public FileResolutionModel(Guid uniqueId)
        {
            UniqueId = uniqueId;
        }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public int IdResolution { get; set; }

        public int? IdResolutionFile { get; set; }

        public int? IdProposalFile { get; set; }

        public int? IdAttachments { get; set; }

        public int? IdControllerFile { get; set; }

        public int? IdAssumedProposal { get; set; }

        public int? IdFrontespizio { get; set; }

        public int? IdPrivacyAttachments { get; set; }

        public int? IdFrontalinoRitiro { get; set; }

        public Guid? IdAnnexes { get; set; }

        public int? IdPrivacyPublicationDocument { get; set; }

        public Guid? IdMainDocumentsOmissis { get; set; }

        public Guid? IdAttachmentsOmissis { get; set; }

        public int? IdUltimaPaginaFile { get; set; }

        public int? IdSupervisoryBoardFile { get; set; }

        public Guid? DematerialisationChainId { get; set; }

        #endregion
    }
}
