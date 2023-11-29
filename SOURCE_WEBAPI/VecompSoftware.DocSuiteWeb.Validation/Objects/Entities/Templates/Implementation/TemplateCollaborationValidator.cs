using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
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

        public TemplateCollaborationValidator(ILogger logger, ITemplateCollaborationValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentsecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentsecurity, parameterEnvSecurity) { }
        #endregion

        #region [ Properties - Document+Folder ]
        public Guid UniqueId { get; set; }
        public TemplateCollaborationRepresentationType RepresentationType { get; set; }
        public string Name { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string TemplateCollaborationPath { get; set; }
        public short TemplateCollaborationLevel { get; set; }
        public Guid? ParentInsertId { get; set; }
        #endregion

        #region [ Properties Document]
        public TemplateCollaborationStatus Status { get; set; }
        public string DocumentType { get; set; }
        public string IdPriority { get; set; }
        public string Object { get; set; }
        public string Note { get; set; }
        public bool? IsLocked { get; set; }
        public bool WSManageable { get; set; }
        public bool WSDeletable { get; set; }
        public string JsonParameters { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public ICollection<TemplateCollaborationUser> TemplateCollaborationUsers { get; set; }
        public ICollection<TemplateCollaborationDocumentRepository> TemplateCollaborationDocumentRepositories { get; set; }
        public ICollection<Role> Roles { get; set; }
        #endregion
    }
}
