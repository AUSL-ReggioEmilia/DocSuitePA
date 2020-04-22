using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions
{
    public class MapperFileResolutionModel : BaseEntityMapper<FileResolution, FileResolutionModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public MapperFileResolutionModel() : base()
        {
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<FileResolution, FileResolution> MappingProjection(IQueryOver<FileResolution, FileResolution> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override FileResolutionModel TransformDTO(FileResolution entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare FileResolution se l'entità non è inizializzata");

            FileResolutionModel model = new FileResolutionModel(entity.UniqueId)
            {
                IdResolution = entity.Id,
                IdResolutionFile = entity.IdResolutionFile,
                IdProposalFile = entity.IdProposalFile,
                IdAttachments = entity.IdAttachements,
                IdControllerFile = entity.IdControllerFile,
                IdAssumedProposal = entity.IdAssumedProposal,
                IdFrontalinoRitiro = entity.IdFrontalinoRitiro,
                IdFrontespizio = entity.IdFrontespizio,
                IdPrivacyAttachments = entity.IdPrivacyAttachments,
                IdAnnexes = entity.IdAnnexes,
                IdPrivacyPublicationDocument = entity.IdPrivacyPublicationDocument,
                IdMainDocumentsOmissis = entity.IdMainDocumentsOmissis,
                IdAttachmentsOmissis = entity.IdAttachmentsOmissis,
                IdUltimaPaginaFile = entity.IdUltimaPagina,
                IdSupervisoryBoardFile = entity.IdSupervisoryBoardFile,
                DematerialisationChainId = entity.DematerialisationChainId
            };

            return model;
        }

        #endregion
    }
}
