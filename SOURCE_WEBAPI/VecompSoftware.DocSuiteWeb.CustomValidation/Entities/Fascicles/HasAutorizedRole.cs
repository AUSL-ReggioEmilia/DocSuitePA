using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasAutorizedRole : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasAutorizedRole(NameValueCollection attributes)
            : base("Nel fascicolo di attività deve essere indicato almeno un settore autorizzato.", nameof(HasAutorizedRole))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            if (objectToValidate.FascicleRoles == null || !objectToValidate.FascicleRoles.Any() || objectToValidate.FascicleRoles.Any(x => x.AuthorizationRoleType != AuthorizationRoleType.Responsible))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
