using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Templates
{
    public class TemplateCollaborationModel
    {
        #region [ Constructor ]
        public TemplateCollaborationModel()
        {
        }
        #endregion

        #region [ Properties ]

        public Guid? UniqueId { get; set; }
        public string Name { get; set; }
        public string DocumentType { get; set; }
        public string IdPriority { get; set; }
        public TemplateCollaborationStatus Status { get; set; }
        public string Object { get; set; }
        public bool? IsLocked { get; set; }
        public string Note { get; set; }
        public bool WSDeletable { get; set; }
        public bool WSManageable { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset? RegistrationDate { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string LastChangedUser { get; set; }

        #endregion
    }
}
