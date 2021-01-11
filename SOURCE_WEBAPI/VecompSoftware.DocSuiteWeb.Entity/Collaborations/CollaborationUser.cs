using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Collaborations
{
    public class CollaborationUser : DSWBaseEntity
    {
        #region [ Constructor ]

        public CollaborationUser() : this(Guid.NewGuid()) { }

        public CollaborationUser(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Get or set Incremental
        /// </summary>
        public short Incremental { get; set; }

        /// <summary>
        /// Get or set DestinationFirst
        /// </summary>
        public bool? DestinationFirst { get; set; }

        /// <summary>
        /// Get or set DestinationType
        /// </summary>
        public string DestinationType { get; set; }

        /// <summary>
        /// Get or set DestinationName
        /// </summary>
        public string DestinationName { get; set; }

        /// <summary>
        /// Get or set DestinationEmail
        /// </summary>
        public string DestinationEmail { get; set; }

        /// <summary>
        /// Get or set Account
        /// </summary>
        public string Account { get; set; }


        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Get or set Collaboration reference
        /// </summary>
        public virtual Collaboration Collaboration { get; set; }

        /// <summary>
        /// Get or set Role reference
        /// </summary>
        public virtual Role Role { get; set; }
        #endregion
    }
}
