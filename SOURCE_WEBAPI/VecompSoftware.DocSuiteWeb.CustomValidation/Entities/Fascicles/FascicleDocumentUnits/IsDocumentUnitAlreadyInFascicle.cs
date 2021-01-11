using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleDocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsDocumentUnitAlreadyInFascicle : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {

        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsDocumentUnitAlreadyInFascicle(NameValueCollection attributes)
          : base("L'atto selezionato è già presente nel fascicolo indicato.", nameof(IsDocumentUnitAlreadyInFascicle))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleDocumentUnitValidator objectToValidate)
        {
            int count = 0;

            count = objectToValidate.Fascicle == null || objectToValidate.DocumentUnit == null ? 1 :
                CurrentUnitOfWork.Repository<FascicleDocumentUnit>().CountByFascicleAndDocumentUnit(objectToValidate.DocumentUnit.UniqueId, objectToValidate.Fascicle.UniqueId);

            if (count > 0)
            {
                GenerateInvalidateResult();
            }
        }

        #endregion


    }
}
