using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.PECMails;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.PECMails
{
    public class PECMailBoxUserDao : BaseNHibernateDao<PECMailBoxUser>
    {
        #region [ Fields ]
        private SecurityUsers securityUser = null;
        private PECMailBox mailBox = null;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public PECMailBoxUserDao() : base()
        {
        }

        public PECMailBoxUserDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }
        #endregion        

        #region [ Methods ]

        public PECMailBoxUser GetUser(string userName, int idMailBox)
        {
            return NHibernateSession.QueryOver<PECMailBoxUser>()
                .JoinAlias(j => j.PECMailBox, () => mailBox)
                .Left.JoinAlias(j => j.SecurityUser, () => securityUser)
                .Where(x => x.AccountName == userName || securityUser.Account == userName)
                .And(() => mailBox.Id == idMailBox)
                .SingleOrDefault<PECMailBoxUser>();
        }

        public ICollection<PECMailBoxUser> GetUsers(int idMailBox)
        {
            return NHibernateSession.QueryOver<PECMailBoxUser>()
                .JoinAlias(j => j.PECMailBox, () => mailBox)
                .Where(x => mailBox.Id == idMailBox)
                .List<PECMailBoxUser>();
        }
        #endregion
    }
}
