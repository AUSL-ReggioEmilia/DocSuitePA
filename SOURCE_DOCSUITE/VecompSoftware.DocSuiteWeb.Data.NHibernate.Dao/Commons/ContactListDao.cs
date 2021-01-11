using NHibernate.Criterion;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons
{
    public class ContactListDao : BaseNHibernateDao<ContactList>
    {
        #region [ Fields ]
        
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public ContactListDao() : base()
        {
        }

        public ContactListDao(string sessionFactoryName) : base(sessionFactoryName)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        public IList<ContactList> GetByName(string name)
        {
            IList<ContactList> results = NHibernateSession.QueryOver<ContactList>()
                .Where(Restrictions.InsensitiveLike("Name", name, MatchMode.Anywhere))
                .List<ContactList>();
            return results;
        }


        #endregion [ Methods ]
    }
}
