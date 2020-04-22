using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.UDS
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsActive : BaseValidator<UDSTypology, UDSTypologyValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsActive(NameValueCollection attributes)
            : base("Lo stato della tipologia deve essere attivo.", nameof(IsActive))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(UDSTypologyValidator objectToValidate)
        {
            if (objectToValidate.Status != UDSTypologyStatus.Active)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
