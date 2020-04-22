using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS
{
    public class UDSDocumentUnitValidator : ObjectValidator<UDSDocumentUnit, UDSDocumentUnitValidator>, IUDSDocumentUnitValidator
    {

        #region [ Constructor ]
        public UDSDocumentUnitValidator(ILogger logger, IUDSDocumentUnitValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity security)
            : base(logger, mapper, unitOfWork, security) { }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public Guid IdUDS { get; set; }

        public int Environment { get; set; }

        public UDSRelationType UDSRelationType { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public UDSRepository Repository { get; set; }

        public DocumentUnit DocumentUnit { get; set; }
        #endregion
    }
}
