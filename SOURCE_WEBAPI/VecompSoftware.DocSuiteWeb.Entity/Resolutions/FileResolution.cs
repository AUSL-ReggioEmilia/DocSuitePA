using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Resolutions
{
    /// <summary>
    /// TODO: La Resolution è da estendere con tutte le proprietà e navigation properties mancanti
    /// </summary>
    public class FileResolution : DSWBaseEntity
    {
        #region [ Constructor ]
        public FileResolution() : this(Guid.NewGuid()) { }
        public FileResolution(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]

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

        #region [ Navigation Properties ]

        public virtual Resolution Resolution { get; set; }

        #endregion


    }
}
