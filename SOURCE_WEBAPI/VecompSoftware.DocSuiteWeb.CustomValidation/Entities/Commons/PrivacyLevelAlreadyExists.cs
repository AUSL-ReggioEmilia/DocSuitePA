using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class PrivacyLevelAlreadyExists : BaseValidator<PrivacyLevel, PrivacyLevelValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public PrivacyLevelAlreadyExists(NameValueCollection attributes)
            : base("Il livello di riservatezza deve essere univoco.", nameof(PrivacyLevelAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(PrivacyLevelValidator objectToValidate)
        {
            int res = CurrentUnitOfWork.Repository<PrivacyLevel>().CountByLevel(objectToValidate.Level, objectToValidate.UniqueId);

            if (res > 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
