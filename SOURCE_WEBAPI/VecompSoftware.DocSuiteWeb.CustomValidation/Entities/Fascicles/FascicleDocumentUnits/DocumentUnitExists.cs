using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;
namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleDocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class DocumentUnitExists : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {


        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public DocumentUnitExists(NameValueCollection attributes)
           : base("Non è stato possibile recuperare il document unit selezionato.", nameof(DocumentUnitExists))
        {
        }
        #endregion

        #region [ Methods ]

        protected override void ValidateObject(FascicleDocumentUnitValidator objectToValidate)
        {
            int count = objectToValidate.DocumentUnit == null ? 0 : CurrentUnitOfWork.Repository<DocumentUnit>().CountByUniqueId(objectToValidate.DocumentUnit.UniqueId);

            if (count == 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
