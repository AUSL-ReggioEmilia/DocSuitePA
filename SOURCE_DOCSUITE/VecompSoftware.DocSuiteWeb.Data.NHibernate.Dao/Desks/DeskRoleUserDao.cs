using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks
{
    public class DeskRoleUserDao : BaseNHibernateDao<DeskRoleUser>
    {
        #region [ Fields ]
        
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskRoleUserDao() : base()
        {
        }

        public DeskRoleUserDao(string sessionFactoryName) : base(sessionFactoryName)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]

        /// <summary>
        /// Verifica se esiste almeno un utente con diritti di approvazione
        /// escludendo l'utente che ha aperto il tavolo
        /// </summary>
        public bool HasUsersApprovers(Guid idDesk)
        {
            Desk desk = null;

            return NHibernateSession.QueryOver<DeskRoleUser>()
                .JoinAlias(x => x.Desk, () => desk)
                .Where(() => desk.Id == idDesk)
                .AndRestrictionOn(x => x.PermissionType)
                .IsIn(new[] { DeskPermissionType.Approval, DeskPermissionType.Manage })
                .RowCount() > 0;
        }

        public DeskRoleUser GetByAccountName(string accountName, Guid idDesk)
        {
            Desk desk = null;

            return NHibernateSession.QueryOver<DeskRoleUser>()
                .JoinAlias(x => x.Desk, () => desk)
                .Where(() => desk.Id == idDesk)
                .And(x => x.AccountName == accountName)
                .SingleOrDefault<DeskRoleUser>();
        }
        #endregion [ Methods ]
    }
}
