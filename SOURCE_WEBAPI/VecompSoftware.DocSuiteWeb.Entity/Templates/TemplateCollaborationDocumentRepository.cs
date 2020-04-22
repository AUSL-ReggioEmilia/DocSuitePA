using System;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Entity.Templates
{
    public class TemplateCollaborationDocumentRepository : DSWBaseEntity
    {
        #region [ Constructor ]
        public TemplateCollaborationDocumentRepository() : this(Guid.NewGuid()) { }
        public TemplateCollaborationDocumentRepository(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]
        public ChainType ChainType { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual TemplateCollaboration TemplateCollaboration { get; set; }
        public virtual TemplateDocumentRepository TemplateDocumentRepository { get; set; }
        #endregion
    }
}
