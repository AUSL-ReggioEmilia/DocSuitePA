using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class ProtocolDocumentTypeValidator : ObjectValidator<ProtocolDocumentType, ProtocolDocumentTypeValidator>, IProtocolDocumentTypeValidator
    {
        #region [ Constructor ]
        public ProtocolDocumentTypeValidator(ILogger logger, IProtocolDocumentTypeValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region[ Properties ]

        public short EntityShortId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public string HiddenFields { get; set; }
        public string NeedPackage { get; set; }
        public string CommonUser { get; set; }

        #endregion

        #region[ Navigation Properties ]
        public ICollection<Protocol> Protocols { get; set; }
        #endregion

    }
}
