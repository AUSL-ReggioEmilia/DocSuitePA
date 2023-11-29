using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits
{
    public class DocumentUnitFascicleCategoryValidator : ObjectValidator<DocumentUnitFascicleCategory, DocumentUnitFascicleCategoryValidator>, IDocumentUnitFascicleCategoryValidator
    {
        #region [ Constructor ]

        public DocumentUnitFascicleCategoryValidator(ILogger logger, IValidatorMapper<DocumentUnitFascicleCategory, DocumentUnitFascicleCategoryValidator> mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity) 
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
        }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public DocumentUnit DocumentUnit { get; set; }
        public Category Category { get; set; }
        public Fascicle Fascicle { get; set; }

        #endregion
    }
}
