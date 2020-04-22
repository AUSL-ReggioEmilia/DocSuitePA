using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Messages
{

    public class MessageContactEmail : DSWBaseEntity
    {
        #region [ Constructor ]
        public MessageContactEmail() : this(Guid.NewGuid()) { }

        public MessageContactEmail(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Utente
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// Indirizzo mail
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Descrizione del contatto della mail
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Messaggio al quale fa riferimento la mail
        /// </summary>
        public virtual MessageContact MessageContact { get; set; }
        #endregion
    }
}
