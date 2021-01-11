using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates
{
    public class TemplateCollaborationValidator : ObjectValidator<TemplateCollaboration, TemplateCollaborationValidator>, ITemplateCollaborationValidator
    {
        #region [ Constructor ]

        public TemplateCollaborationValidator(ILogger logger, ITemplateCollaborationValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentsecurity)
            : base(logger, mapper, unitOfWork, currentsecurity) { }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string Name { get; set; }
        public TemplateCollaborationStatus Status { get; set; }
        public string DocumentType { get; set; }
        public string IdPriority { get; set; }
        public string Object { get; set; }
        public string Note { get; set; }
        public bool? IsLocked { get; set; }
        public bool WSManageable { get; set; }
        public bool WSDeletable { get; set; }
        public string JsonParameters { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public ICollection<TemplateCollaborationUser> TemplateCollaborationUsers { get; set; }
        public ICollection<TemplateCollaborationDocumentRepository> TemplateCollaborationDocumentRepositories { get; set; }
        public ICollection<Role> Roles { get; set; }
        #endregion
    }
}
