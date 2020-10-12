using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsLocationNameUnique : BaseValidator<Location, LocationValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public IsLocationNameUnique(NameValueCollection attributes)
            : base("Un deposito documentale con il nome scelto è già esistente.", nameof(IsLocationNameUnique))
        {
        }

        #endregion

        #region [ Methods ]

        protected override void ValidateObject(LocationValidator objectToValidate)
        {
            bool result = true;
            if (!string.IsNullOrEmpty(objectToValidate.Name))
            {
                result = CurrentUnitOfWork.Repository<Location>().NameAlreadyExists(objectToValidate.Name, objectToValidate.UniqueId);
            }
            if (result)
            {
                GenerateInvalidateResult();
            }
        }

        #endregion
    }
}
