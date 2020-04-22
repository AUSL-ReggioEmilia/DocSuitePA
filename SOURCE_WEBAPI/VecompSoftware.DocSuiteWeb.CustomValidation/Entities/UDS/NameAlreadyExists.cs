using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.UDS
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class NameAlreadyExists : BaseValidator<UDSTypology, UDSTypologyValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public NameAlreadyExists(NameValueCollection attributes)
            : base("Il nome della tipologia deve essere univoco.", nameof(NameAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(UDSTypologyValidator objectToValidate)
        {
            bool result = (string.IsNullOrEmpty(objectToValidate.Name) || objectToValidate.UniqueId == null) ? true : CurrentUnitOfWork.Repository<UDSTypology>().NameAlreadyExists(objectToValidate.Name, objectToValidate.UniqueId);

            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}

