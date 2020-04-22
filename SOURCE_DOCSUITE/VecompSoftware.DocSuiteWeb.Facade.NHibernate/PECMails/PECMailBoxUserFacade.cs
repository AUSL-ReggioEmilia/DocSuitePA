using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.PECMails;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.PECMails
{
    public class PECMailBoxUserFacade : BaseProtocolFacade<PECMailBoxUser, Guid, PECMailBoxUserDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public PECMailBoxUserFacade(string userName)
            : base()
        {
            _userName = userName;
        }
        #endregion        

        #region [ Methods ]

        /// <summary>
        /// Verifica se un determinato account, per una determinata casella PEC, esiste
        /// </summary>
        /// <param name="userName">Nome utente</param>
        /// <param name="idMailBox">Id PecMailBox</param>
        /// <returns></returns>
        public bool UserExist(string userName, int idMailBox)
        {
            PECMailBoxUser user = _dao.GetUser(userName, idMailBox);
            return user != null;
        }
        /// <summary>
        /// Restituisce l'elenco, se esiste, degli utenti abilitati alla notifica della casella di posta (PECMailBox)
        /// </summary>
        /// <param name="idMailBox">Id PecMailBox</param>
        /// <returns></returns>
        public ICollection<PECMailBoxUser> GetUsers(int idMailBox)
        {
            return _dao.GetUsers(idMailBox) ?? new List<PECMailBoxUser>();
        }
        #endregion
    }
}
