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
    public class DocumentUnitContactValidator : ObjectValidator<DocumentUnitContact, DocumentUnitContactValidator>, IDocumentUnitContactValidator
    {
        #region [ Constructor ]
        public DocumentUnitContactValidator(ILogger logger, IDocumentUnitContactValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        public string ContactManual { get; set; }
        public short ContactType { get; set; }
        public string ContactLabel { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual Contact Contact { get; set; }
        public virtual DocumentUnit DocumentUnit { get; set; }
        #endregion
    }
}
