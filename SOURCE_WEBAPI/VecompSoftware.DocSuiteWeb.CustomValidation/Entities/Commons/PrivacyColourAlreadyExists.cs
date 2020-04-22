using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class PrivacyColourAlreadyExists : BaseValidator<PrivacyLevel, PrivacyLevelValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public PrivacyColourAlreadyExists(NameValueCollection attributes)
            : base("Il colore del livello di riservatezza deve essere univoco.", nameof(PrivacyColourAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(PrivacyLevelValidator objectToValidate)
        {
            bool res = !string.IsNullOrEmpty(objectToValidate.Colour) && CurrentUnitOfWork.Repository<PrivacyLevel>().CountByColour(objectToValidate.Colour, objectToValidate.UniqueId) > 0;

            if (res)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
