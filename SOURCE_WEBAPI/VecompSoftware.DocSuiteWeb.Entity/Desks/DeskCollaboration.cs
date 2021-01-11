using System;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{

    public class DeskCollaboration : DSWBaseEntity
    {
        #region [ Constructor ]
        public DeskCollaboration() : this(Guid.NewGuid()) { }

        public DeskCollaboration(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Oggetto esterno riferito ad una collaborazione
        /// </summary>
        public virtual Collaboration Collaboration { get; set; }

        /// <summary>
        /// Oggetto esterno riferito ad un tavolo
        /// </summary>
        public virtual Desk Desk { get; set; }
        #endregion
    }
}
