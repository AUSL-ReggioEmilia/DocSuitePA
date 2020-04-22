using System;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Collaborations
{
    public class CollaborationVersioning : DSWBaseEntity, IEntityLogicDelete
    {
        #region [ Constructor ]

        public CollaborationVersioning() : this(Guid.NewGuid()) { }

        public CollaborationVersioning(Guid uniqueId)
            : base(uniqueId) { }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Get or set CollaborationIncremental
        /// </summary>
        public short CollaborationIncremental { get; set; }

        /// <summary>
        /// Get or set Incremental
        /// </summary>
        public short Incremental { get; set; }

        /// <summary>
        /// Get or set IdDocument
        /// </summary>
        public int IdDocument { get; set; }

        /// <summary>
        /// Get or set IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Get or set DocumentName
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        /// Get or set CheckedOut
        /// </summary>
        public bool? CheckedOut { get; set; }

        /// <summary>
        /// Get or set CheckOutUser
        /// </summary>
        public string CheckOutUser { get; set; }

        /// <summary>
        /// Get or set CheckOutSessionId
        /// </summary>
        public string CheckOutSessionId { get; set; }

        /// <summary>
        /// Get or set CheckOutDate
        /// </summary>
        public DateTimeOffset? CheckOutDate { get; set; }

        /// <summary>
        /// Get or set DocumentChecksum
        /// </summary>
        public string DocumentChecksum { get; set; }

        /// <summary>
        /// Get or set DocumentGroup
        /// </summary>
        public string DocumentGroup { get; set; }


        #endregion

        #region [ Navigation Properties ]

        public virtual Collaboration Collaboration { get; set; }
        #endregion
    }
}
