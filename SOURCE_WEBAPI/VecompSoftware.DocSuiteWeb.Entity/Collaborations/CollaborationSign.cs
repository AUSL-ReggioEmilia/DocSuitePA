using System;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Collaborations
{
    public class CollaborationSign : DSWBaseEntity, IEntityLogicDelete
    {
        #region [ Constructor ]

        public CollaborationSign() : this(Guid.NewGuid()) { }

        public CollaborationSign(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Get or set Incremental
        /// </summary>
        public short Incremental { get; set; }

        /// <summary>
        /// Get or set IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Get or set IdStatus
        /// </summary>
        public string IdStatus { get; set; }

        /// <summary>
        /// Get or set SignUser
        /// </summary>
        public string SignUser { get; set; }

        /// <summary>
        /// Get or set SignName
        /// </summary>
        public string SignName { get; set; }

        /// <summary>
        /// Get or set SignEmail
        /// </summary>
        public string SignEmail { get; set; }

        /// <summary>
        /// Get or set SignDate
        /// </summary>
        public DateTime? SignDate { get; set; }

        /// <summary>
        /// Get or set IsRequired
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Get or set IsAbsent
        /// </summary>
        public bool? IsAbsent { get; set; }
        #endregion

        #region [ Navigation Properties ]

        public virtual Collaboration Collaboration { get; set; }
        #endregion
    }
}
