using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks
{
    public class DeskDao : BaseNHibernateDao<Desk>
    {
        #region [ Fields ]
        
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskDao() : base()
        {
        }

        public DeskDao(string sessionFactoryName) : base(sessionFactoryName)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        public Desk GetByIdCollaboration(int idCollaboration)
        {
            Collaboration collaboration = null;
            Desk desk = null;

            Desk result = NHibernateSession.QueryOver<DeskCollaboration>()
                                           .JoinAlias(x => x.Collaboration, () => collaboration)
                                           .JoinAlias(x => x.Desk, () => desk)
                                           .Where(() => collaboration.Id == idCollaboration)
                                           .Select(s => s.Desk)
                                           .SingleOrDefault<Desk>();
            return result;
        }
        #endregion [ Methods ]
    }
}
