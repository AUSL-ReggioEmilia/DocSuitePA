using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions
{
    public class FileResolutionValidator : ObjectValidator<FileResolution, FileResolutionValidator>, IFileResolutionValidator
    {
        #region [ Constructor ]
        public FileResolutionValidator(ILogger logger, IFileResolutionValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
        }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public int EntityId { get; set; }

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

        public Resolution Resolution { get; set; }

        #endregion

    }
}
