using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleDocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsLegacyFascicle : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {


        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsLegacyFascicle(NameValueCollection attributes)
           : base("Non è possibile inserire unità documentarie in Fascicoli non a norma.", nameof(IsLegacyFascicle))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleDocumentUnitValidator objectToValidate)
        {
            if (objectToValidate.Fascicle != null && objectToValidate.ReferenceType == ReferenceType.Fascicle && objectToValidate.Fascicle.FascicleType == FascicleType.Legacy)
            {
                GenerateInvalidateResult();
            }
        }

        #endregion
    }
}
