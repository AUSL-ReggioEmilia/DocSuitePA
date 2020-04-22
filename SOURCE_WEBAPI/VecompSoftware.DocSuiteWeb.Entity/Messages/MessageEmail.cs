using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Entity.Messages
{
    public class MessageEmail : DSWBaseEntity
    {
        #region [ Constructor ]
        public MessageEmail() : this(Guid.NewGuid()) { }

        public MessageEmail(Guid uniqueId)
            : base(uniqueId)
        {
            DeskMessages = new HashSet<DeskMessage>();
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Data di invio
        /// </summary>
        public DateTime? SentDate { get; set; }
        /// <summary>
        /// Oggetto del messaggio
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Corpo del messaggio
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Priorità
        /// </summary>
        public string Priority { get; set; }
        /// <summary>
        /// Guid del documento EML
        /// </summary>
        public Guid? EmlDocumentId { get; set; }
        /// <summary>
        /// E' un messaggio di notifica
        /// </summary>
        public bool? IsDispositionNotification { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<DeskMessage> DeskMessages { get; set; }

        /// <summary>
        /// Messaggio al quale fa riferimento la mail
        /// </summary>
        public virtual Message Message { get; set; }
        #endregion
    }
}
