using System;
using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{

    public class DeskMessage : DSWBaseEntity
    {
        #region [ Constructor ]

        public DeskMessage() : this(Guid.NewGuid()) { }

        public DeskMessage(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]  

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Riferimento al "Tavolo"
        /// </summary>
        public virtual Desk Desk { get; set; }
        /// <summary>
        /// Riferimento al "Message"
        /// </summary>
        public virtual MessageEmail Message { get; set; }
        #endregion
    }
}
