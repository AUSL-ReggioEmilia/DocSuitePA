using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks
{
    public class DeskDocumentEndorsementDao : BaseNHibernateDao<DeskDocumentEndorsement>
    {
        #region [ Fields ]
        
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskDocumentEndorsementDao() : base()
        {
        }

        public DeskDocumentEndorsementDao(string sessionFactoryName) : base(sessionFactoryName)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        public ICollection<DeskDocumentEndorsement> GetByIdVersionAndAccount(Guid idDeskDocumentVersion, string account)
        {
            DeskRoleUser deskRoleUser = null;

            return NHibernateSession.QueryOver<DeskDocumentEndorsement>()
                .Where(x => x.DeskDocumentVersion.Id == idDeskDocumentVersion)
                .JoinAlias(x => x.DeskRoleUser, () => deskRoleUser)
                .AndRestrictionOn(() => deskRoleUser.AccountName).IsInsensitiveLike(account)
                .List<DeskDocumentEndorsement>();
        }
        #endregion [ Methods ]
    }
}
