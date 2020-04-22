using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleDocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsPeriodicFascicle : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {


        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsPeriodicFascicle(NameValueCollection attributes)
            : base("Non è possibile rimuovere unità documentarie fascicolate in Fascicoli periodici.", nameof(IsPeriodicFascicle))
        {
        }

        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleDocumentUnitValidator objectToValidate)
        {
            if (objectToValidate.Fascicle != null && objectToValidate.ReferenceType == ReferenceType.Fascicle && objectToValidate.Fascicle.FascicleType == FascicleType.Period)
            {
                GenerateInvalidateResult();
            }
        }

        #endregion


    }
}
