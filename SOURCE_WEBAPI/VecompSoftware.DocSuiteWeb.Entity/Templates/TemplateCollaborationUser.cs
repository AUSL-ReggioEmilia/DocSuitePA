using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Templates
{
    public class TemplateCollaborationUser : DSWBaseEntity
    {
        #region [ Contructor ]
        public TemplateCollaborationUser() : this(Guid.NewGuid()) { }
        public TemplateCollaborationUser(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public string Account { get; set; }
        public short Incremental { get; set; }
        public TemplateCollaborationUserType UserType { get; set; }
        public bool IsRequired { get; set; }
        public bool IsValid { get; set; }
        #endregion

        #region [ Navigation Properies]
        public virtual TemplateCollaboration TemplateCollaboration { get; set; }
        public virtual Role Role { get; set; }
        #endregion
    }
}
