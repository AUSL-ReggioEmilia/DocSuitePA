using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsActivityFascicleReserved : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsActivityFascicleReserved(NameValueCollection attributes)
            : base("Non è possibile inserire un fascicolo di attività non riservato.", nameof(IsActivityFascicleReserved))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            if (objectToValidate.VisibilityType != VisibilityType.Confidential)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
