using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Desks
{
    public class MapperDeskEndorsment : BaseEntityMapper<DeskDocumentEndorsement, DeskEndorsement>
    {
        #region [ Constructor ]
        public MapperDeskEndorsment() : base() { }
        #endregion

        #region [ Methods ]
        protected override DeskEndorsement TransformDTO(DeskDocumentEndorsement entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare DeskEndorsement se l'entità non è inizializzata");

            DeskEndorsement dto = new DeskEndorsement();
            dto.AccountName = entity.DeskRoleUser.AccountName;
            dto.DeskDocumentId = entity.DeskDocumentVersion.DeskDocument.Id;
            dto.IsApprove = entity.Endorsement;
            dto.Version = entity.DeskDocumentVersion.Version;

            return dto;
        }

        protected override IQueryOver<DeskDocumentEndorsement, DeskDocumentEndorsement> MappingProjection(IQueryOver<DeskDocumentEndorsement, DeskDocumentEndorsement> queryOver)
        {
            DeskEndorsement deskEndorsment = null;
            DeskRoleUser deskRoleUser = null;
            DeskDocumentVersion deskDocumentVersion = null;
            DeskDocument deskDocument = null;

            queryOver.JoinAlias(o => o.DeskRoleUser, () => deskRoleUser)
                .SelectList(list => list
                    // Mappatura degli oggetti DeskDocumentEndorsment
                    .Select(x => x.Endorsement).WithAlias(() => deskEndorsment.IsApprove)
                    // Mappatura degli oggetti DeskDocumentVersion
                    .Select(() => deskDocumentVersion.Version).WithAlias(() => deskEndorsment.Version)
                    // Mappatura degli oggetti DeskDocument
                    .Select(() => deskDocument.Id).WithAlias(() => deskEndorsment.DeskDocumentId)
                    .Select(() => deskRoleUser.AccountName).WithAlias(() => deskEndorsment.AccountName));

            return queryOver;
        }

        #endregion        
    }
}
