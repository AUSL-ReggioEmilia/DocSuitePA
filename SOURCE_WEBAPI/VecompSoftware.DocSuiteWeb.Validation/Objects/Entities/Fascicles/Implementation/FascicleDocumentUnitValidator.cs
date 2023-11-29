using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles
{
    public class FascicleDocumentUnitValidator : ObjectValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>, IFascicleDocumentUnitValidator
    {
        public FascicleDocumentUnitValidator(ILogger logger, IFascicleDocumentUnitValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
        }

        #region [Properties]

        public Guid UniqueId { get; set; }

        /// <summary>
        /// Get or set ReferenceType
        /// </summary>
        public ReferenceType ReferenceType { get; set; }
 
        /// <summary>
        /// Get or set SequenceNumber
        /// </summary>
        public short SequenceNumber { get; set; }
        
        /// <summary>
        /// Get or set RegistrationUser
        /// </summary>
        public string RegistrationUser { get; set; }

        /// <summary>
        /// Get or set RegistrationDate
        /// </summary>
        public DateTimeOffset RegistrationDate { get; set; }

        /// <summary>
        /// Get or set LastChangedUser
        /// </summary>
        public string LastChangedUser { get; set; }

        /// <summary>
        /// Get or set LastChangedDate
        /// </summary>
        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Fascicle Fascicle { get; set; }
        public FascicleFolder FascicleFolder { get; set; }
        public DocumentUnit DocumentUnit { get; set; }

        #endregion
    }
}
