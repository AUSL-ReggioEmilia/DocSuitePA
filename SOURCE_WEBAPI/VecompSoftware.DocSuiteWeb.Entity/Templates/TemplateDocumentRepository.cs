using System;
using System.Collections.Generic;


namespace VecompSoftware.DocSuiteWeb.Entity.Templates
{
    public class TemplateDocumentRepository : DSWBaseEntity
    {
        #region [ Constructor ]

        public TemplateDocumentRepository() : this(Guid.NewGuid()) { }

        public TemplateDocumentRepository(Guid uniqueId)
           : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Get or set Status
        /// </summary>
        public TemplateDocumentationRepositoryType Status { get; set; }

        /// <summary>
        /// Get or set Object
        /// </summary>
        public string Object { get; set; }

        /// <summary>
        /// Get or set NameOfTemplate
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set QualityTag
        /// </summary>
        public string QualityTag { get; set; }

        /// <summary>
        /// Get or set Version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Get or set IdArchiveChain
        /// </summary>
        public Guid IdArchiveChain { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<TemplateCollaborationDocumentRepository> TemplateCollaborationDocumentRepositories { get; set; }
        #endregion
    }
}
