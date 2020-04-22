using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsFascicleManagerUnique : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsFascicleManagerUnique(NameValueCollection attributes)
            : base("Il responsabile di procedimento deve essere unico.", nameof(IsFascicleManagerUnique))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            if (objectToValidate.Contacts == null || !objectToValidate.Contacts.Any() || objectToValidate.Contacts.Count > 1)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}