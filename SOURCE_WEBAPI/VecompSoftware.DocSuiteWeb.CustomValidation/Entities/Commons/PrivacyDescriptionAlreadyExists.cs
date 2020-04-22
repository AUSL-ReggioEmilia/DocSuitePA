using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class PrivacyDescriptionAlreadyExists : BaseValidator<PrivacyLevel, PrivacyLevelValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public PrivacyDescriptionAlreadyExists(NameValueCollection attributes)
            : base("L'etichetta del livello di riservatezza deve essere univoca.", nameof(PrivacyDescriptionAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(PrivacyLevelValidator objectToValidate)
        {
            bool res = string.IsNullOrEmpty(objectToValidate.Description) || CurrentUnitOfWork.Repository<PrivacyLevel>().CountByDescription(objectToValidate.Description, objectToValidate.UniqueId) > 0;

            if (res)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
