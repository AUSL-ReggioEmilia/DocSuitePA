using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks
{
    public class DeskStoryBoardDao : BaseNHibernateDao<DeskStoryBoard>
    {
        #region [ Fields ]
        
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskStoryBoardDao() : base()
        {
        }

        public DeskStoryBoardDao(string sessionFactoryName) : base(sessionFactoryName)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]
        public DeskStoryBoard GetLastStoryBoard(Guid idDeskDocumentVersion)
        {
            DeskDocumentVersion deskDocumentVersion = null;

            return NHibernateSession.QueryOver<DeskStoryBoard>()
                .JoinAlias(x => x.DeskDocumentVersion, () => deskDocumentVersion)
                .Where(() => deskDocumentVersion.Id == idDeskDocumentVersion)
                .OrderBy(o => o.RegistrationDate).Desc
                .Take(1)
                .SingleOrDefault<DeskStoryBoard>();
        }

        #endregion [ Methods ]
    }
}
