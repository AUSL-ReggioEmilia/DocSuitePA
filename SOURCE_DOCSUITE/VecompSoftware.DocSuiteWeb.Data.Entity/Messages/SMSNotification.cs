using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Messages
{
    public class SMSNotification : DomainObject<Guid>, IAuditable
    {
        #region [ Constructor ]
        protected SMSNotification() { }

        public SMSNotification(string userName)
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
        public virtual PECMail PECMail { get; set; }

        /// <summary>
        /// Nome dell'account
        /// </summary>
        public virtual string AccountName { get; set; }

        /// <summary>
        /// Tipo della notifica
        /// </summary>
        public virtual SMSNotificationType NotificationType { get; set; }
        
        /// <summary>
        /// Stato logico della notifica
        /// </summary>
        public virtual LogicalStateType LogicalState { get; set; }

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
