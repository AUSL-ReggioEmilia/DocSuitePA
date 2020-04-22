using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.Messages
{

    public class MessageContact : DSWBaseEntity
    {
        #region [ Constructor ]

        public MessageContact() : this(Guid.NewGuid()) { }

        public MessageContact(Guid uniqueId)
            : base(uniqueId)
        {
            MessageContactEmail = new HashSet<MessageContactEmail>();
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Tipo di contatto
        /// </summary>
        public MessageContactType ContactType { get; set; }
        /// <summary>
        /// Contatto di posizione
        /// <example>Sender, Recipient, RecipientCc, RecipientBcc</example>
        /// </summary>
        public MessageContactPosition ContactPosition { get; set; }
        /// <summary>
        /// Descrizione della mail
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Messaggio in cui è contenuto l'attachment
        /// </summary>
        public virtual Message Message { get; set; }

        public virtual ICollection<MessageContactEmail> MessageContactEmail { get; set; }
        #endregion
    }
}
