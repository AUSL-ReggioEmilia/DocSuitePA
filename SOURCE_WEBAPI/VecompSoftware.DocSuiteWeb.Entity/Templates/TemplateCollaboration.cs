using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Templates
{
    /**
   * The template collaboration model is desigen to be represented in as a tree model with information
   * for both the folder and the template representable by the same entity.
   * 
   * When a folder is represented the RepresentationType will be Folder and only folder properties are
   * considered.
   * 
   * When a template is represented the RepresentationType will be Template (default value) and folder specific
   * properties are considered but they represent the edge node and can have no more children.
   */
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

        #region [ Properties - Document+Folder ]

        public string Name { get; set; }

        /// <summary>
        /// Determines if the entry is a folder or a template
        /// </summary>
        public TemplateCollaborationRepresentationType RepresentationType { get; set; }

        /// <summary>
        /// Procedure computed property
        /// </summary>
        public string TemplateCollaborationPath { get; set; }

        /// <summary>
        /// Procedure computed property
        /// </summary>
        public short TemplateCollaborationLevel { get; set; }
        /// <summary>
        /// Fake property used to attach insert to proper parent
        /// </summary>
        public Guid? ParentInsertId { get; set; }

        #endregion

        #region [ Properties - Document ]

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
