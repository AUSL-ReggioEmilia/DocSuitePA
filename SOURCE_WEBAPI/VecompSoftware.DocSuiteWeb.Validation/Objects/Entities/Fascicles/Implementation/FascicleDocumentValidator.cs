using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles
{
    public class FascicleDocumentValidator : ObjectValidator<FascicleDocument, FascicleDocumentValidator>, IFascicleDocumentValidator
    {
        #region [ Constructor ]
        public FascicleDocumentValidator(ILogger logger, IFascicleDocumentValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public ChainType ChainType { get; set; }

        public Guid IdArchiveChain { get; set; }

        public byte[] Timestamp { get; set; }


        #endregion

        #region [ Navigation Properties ]
        public Fascicle Fascicle { get; set; }

        public FascicleFolder FascicleFolder { get; set; }
        #endregion
    }
}
