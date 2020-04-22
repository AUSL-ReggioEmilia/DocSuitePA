using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits
{
    public class DocumentUnitFascicleHistoricizedCategoryValidator : ObjectValidator<DocumentUnitFascicleHistoricizedCategory, DocumentUnitFascicleHistoricizedCategoryValidator>, IDocumentUnitFascicleHistoricizedCategoryValidator
    {
        #region [ Constructor ]

        public DocumentUnitFascicleHistoricizedCategoryValidator(ILogger logger, IValidatorMapper<DocumentUnitFascicleHistoricizedCategory, DocumentUnitFascicleHistoricizedCategoryValidator> mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity) 
            : base(logger, mapper, unitOfWork, currentSecurity)
        {

        }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public DateTimeOffset UnfascicolatedDate { get; set; }
        public string ReferencedFascicle { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public DocumentUnit DocumentUnit { get; set; }
        public Category Category { get; set; }

        #endregion
    }
}
