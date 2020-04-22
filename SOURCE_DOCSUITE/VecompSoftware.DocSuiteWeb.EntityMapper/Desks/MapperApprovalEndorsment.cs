using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Desks
{
    public class MapperApprovalEndorsement : BaseEntityMapper<Desk, DeskEndorsement>
    {
        #region [ Constructor ]
        public MapperApprovalEndorsement() : base() { }
        #endregion

        #region [ Methods ]
        
        protected override DeskEndorsement TransformDTO(Desk entity)
        {
            throw new ArgumentException("Impossibile trasformare DeskEndorsement se l'entità non è inizializzata");
        }
        /// <summary>
        /// Mappo gli oggetti di DeskDocumentEndorsement su DeskEndorsement.
        /// <see cref="IsApprove">IsApprove viene settato intero poichè in una PivotGrid ho la necessità di aggregare questa informazione</see>
        /// </summary>
        /// <param name="queryOver"></param>
        /// <returns></returns>
        protected override IQueryOver<Desk, Desk> MappingProjection(IQueryOver<Desk, Desk> queryOver)
        {
            DeskEndorsement deskEndorsment = null;
            DeskDocumentEndorsement deskDocumentEndorsement = null;
            DeskRoleUser deskRoleUser = null;
            DeskDocumentVersion deskDocumentVersion = null;
            DeskDocument deskDocument = null;

            queryOver
                .SelectList(list => list
                    // Mappatura degli oggetti DeskDocumentEndorsmen
                    //.Select(Projections.Cast(NHibernateUtil.Int32, Projections.Property(() => deskDocumentEndorsement.Endorsement))).WithAlias(() => deskEndorsment.IsApprove)
                    .Select(() => deskDocumentEndorsement.Endorsement).WithAlias(() => deskEndorsment.IsApprove)
                    // Mappatura degli oggetti DeskDocumentVersion
                    .Select(() => deskDocumentVersion.Version).WithAlias(() => deskEndorsment.Version)
                    // Mappatura degli oggetti DeskDocument
                    .Select(() => deskDocument.Id).WithAlias(() => deskEndorsment.DeskDocumentId)
                    .Select(() => deskDocument.IdDocument).WithAlias(() => deskEndorsment.IdChainBiblos)
                    .Select(() => deskRoleUser.AccountName).WithAlias(() => deskEndorsment.AccountName));

            return queryOver;
        }

        #endregion        
    }
}
