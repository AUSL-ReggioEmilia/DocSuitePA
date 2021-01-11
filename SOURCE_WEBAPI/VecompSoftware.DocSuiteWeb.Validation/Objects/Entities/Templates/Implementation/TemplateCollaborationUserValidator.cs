using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates
{
    public class TemplateCollaborationUserValidator : ObjectValidator<TemplateCollaborationUser, TemplateCollaborationUserValidator>, ITemplateCollaborationUserValidator
    {
        #region [ Constructor ]
        public TemplateCollaborationUserValidator(ILogger logger, ITemplateCollaborationUserValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentsecurity)
            : base(logger, mapper, unitOfWork, currentsecurity)
        {
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string Account { get; set; }
        public short Incremental { get; set; }
        public TemplateCollaborationUserType UserType { get; set; }
        public bool IsRequired { get; set; }
        public bool IsValid { get; set; }
        public int IdRole { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public Role Role { get; set; }
        public TemplateCollaboration TemplateCollaboration { get; set; }
        #endregion
    }
}
