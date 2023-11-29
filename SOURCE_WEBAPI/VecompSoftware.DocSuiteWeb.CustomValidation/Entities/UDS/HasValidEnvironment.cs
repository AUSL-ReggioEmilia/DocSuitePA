using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.UDS
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasValidEnvironment : BaseValidator<UDSFieldList, UDSFieldListValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidEnvironment(NameValueCollection attributes)
            : base("L'environment non corrisponde a quello del archivi.", nameof(IsActive))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(UDSFieldListValidator objectToValidate)
        {
            bool isDraftValid = objectToValidate.Environment == 0 && objectToValidate?.Repository.Status == UDSRepositoryStatus.Draft && objectToValidate?.Repository.Version == 0;
            if (objectToValidate.Repository == null || (objectToValidate.Environment < 100 && !isDraftValid))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
