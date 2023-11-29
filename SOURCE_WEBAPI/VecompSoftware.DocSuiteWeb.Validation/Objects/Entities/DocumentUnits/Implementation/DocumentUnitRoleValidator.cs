using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits
{
    public class DocumentUnitRoleValidator : ObjectValidator<DocumentUnitRole, DocumentUnitRoleValidator>, IDocumentUnitRoleValidator
    {
        #region [ Constructor ]
        public DocumentUnitRoleValidator(ILogger logger, IDocumentUnitRoleValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }

        public string RoleLabel { get; set; }

        public Guid UniqueIdRole { get; set; }

        public string AssignUser { get; set; }

        public AuthorizationRoleType RoleAuthorizationType { get; set; }

        #endregion


        #region [ Navigation Properties ]

        public DocumentUnit DocumentUnit { get; set; }

        #endregion
    }
}