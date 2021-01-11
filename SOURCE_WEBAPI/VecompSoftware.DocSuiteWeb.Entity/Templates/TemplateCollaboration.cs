using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Templates
{
    public class TemplateCollaboration : DSWBaseEntity
    {
        #region [ Constructor ]
        public TemplateCollaboration() : this(Guid.NewGuid()) { }
        public TemplateCollaboration(Guid uniqueId)
            : base(uniqueId)
        {
            TemplateCollaborationUsers = new HashSet<TemplateCollaborationUser>();
            TemplateCollaborationDocumentRepositories = new HashSet<TemplateCollaborationDocumentRepository>();
            Roles = new HashSet<Role>();
        }
        #endregion

        #region [ Properties ]

        public string Name { get; set; }
        public TemplateCollaborationStatus Status { get; set; }
        public string DocumentType { get; set; }
        public string IdPriority { get; set; }
        public string Object { get; set; }
        public string Note { get; set; }
        public bool? IsLocked { get; set; }
        public bool WSDeletable { get; set; }
        public bool WSManageable { get; set; }
        public string JsonParameters { get; set; }

        #endregion

        #region [ Navigation Properies]
        public virtual ICollection<TemplateCollaborationUser> TemplateCollaborationUsers { get; set; }
        public virtual ICollection<TemplateCollaborationDocumentRepository> TemplateCollaborationDocumentRepositories { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        #endregion
    }
}
