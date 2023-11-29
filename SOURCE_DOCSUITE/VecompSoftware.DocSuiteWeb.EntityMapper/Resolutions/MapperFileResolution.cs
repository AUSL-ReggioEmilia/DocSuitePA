using System;
using NHibernate;
using APIResolution = VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using DSW = VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions
{
    public class MapperFileResolution : BaseEntityMapper<DSW.FileResolution, APIResolution.FileResolution>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public MapperFileResolution() : base()
        {
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<DSW.FileResolution, DSW.FileResolution> MappingProjection(IQueryOver<DSW.FileResolution, DSW.FileResolution> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIResolution.FileResolution TransformDTO(DSW.FileResolution entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare FileResolution se l'entità non è inizializzata");
            }

            APIResolution.FileResolution model = new APIResolution.FileResolution(entity.UniqueId)
            {
                EntityId = entity.Id,
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
