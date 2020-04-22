using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks
{
    public class DeskDocumentVersionDao : BaseNHibernateDao<DeskDocumentVersion>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        #endregion

        #region [ Methods ]
        public DeskDocumentVersion GetLastVersionByIdDeskDocument(Guid idDeskDocument)
        {
            DeskDocument deskDocument = null;

            return NHibernateSession.QueryOver<DeskDocumentVersion>()
                .JoinAlias(x => x.DeskDocument, () => deskDocument)
                .Where(() => deskDocument.Id == idDeskDocument)
                .OrderBy(o => o.Version)
                .Desc
                .Take(1)
                .SingleOrDefault<DeskDocumentVersion>();
        }

        public DeskDocumentVersion GetByIdAndVersion(Guid idDeskDocument, decimal version)
        {
            return NHibernateSession.QueryOver<DeskDocumentVersion>()
                .Where(x => x.DeskDocument.Id == idDeskDocument)
                .And(x => x.Version == version)
                .SingleOrDefault<DeskDocumentVersion>();
        }
        #endregion
    }
}
