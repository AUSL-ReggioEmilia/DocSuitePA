using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.PECMails
{
    public class PECMailBoxUser : DomainObject<Guid>, IAuditable
    {
        #region [ Constructor ]
        protected PECMailBoxUser() { }

        public PECMailBoxUser(string userName)
            :this()
        {
            this.RegistrationDate = DateTimeOffset.UtcNow;
            this.RegistrationUser = userName;
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Riferimento all'entità di PECMailBox
        /// </summary>
        public virtual PECMailBox PECMailBox { get; set; }

        /// <summary>
        /// Riferimento all'entità di SecurityUsers
        /// </summary>
        public virtual SecurityUsers SecurityUser { get; set; }

        /// <summary>
        /// Nome dell'account
        /// </summary>
        public virtual string AccountName { get; set; }

        /// <summary>
        /// Utente che ha salvato le informazioni
        /// </summary>
        public virtual string RegistrationUser { get; set; }

        /// <summary>
        /// Data creazione delle informazioni
        /// </summary>
        public virtual DateTimeOffset RegistrationDate { get; set; }

        /// <summary>
        /// Utente ultima modifica delle informazioni
        /// </summary>
        public virtual string LastChangedUser { get; set; }

        /// <summary>
        /// Data ultima modifica delle informazioni
        /// </summary>
        public virtual DateTimeOffset? LastChangedDate { get; set; }
        #endregion
    }
}
