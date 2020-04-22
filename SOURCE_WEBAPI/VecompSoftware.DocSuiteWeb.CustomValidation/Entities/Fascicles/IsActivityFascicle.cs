using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsActivityFascicle : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsActivityFascicle(NameValueCollection attributes)
            : base("Il fascicolo dev'essere di tipo Attività.", nameof(IsActivityFascicle))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            if (objectToValidate.FascicleType != FascicleType.Activity)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
