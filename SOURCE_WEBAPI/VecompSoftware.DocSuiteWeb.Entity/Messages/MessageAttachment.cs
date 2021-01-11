using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Messages
{
    public class MessageAttachment : DSWBaseEntity
    {
        #region [ Constructor ]

        public MessageAttachment() : this(Guid.NewGuid()) { }

        public MessageAttachment(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Archive in cui è archiviato l'attacment
        /// </summary>
        public string Archive { get; set; }
        /// <summary>
        /// Id della catena in cui è archiviato l'attacment
        /// </summary>
        public int ChainId { get; set; }
        /// <summary>
        /// Indice posizionale del documento
        /// </summary>
        public int? DocumentEnum { get; set; }
        /// <summary>
        /// Estensione dell'attachment
        /// </summary>
        public string Extension { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Messaggio in cui è contenuto l'attachment
        /// </summary>
        public virtual Message Message { get; set; }
        #endregion
    }
}
