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
    public class HasManagerFiscalCode : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasManagerFiscalCode(NameValueCollection attributes)
            : base("Il contatto del responsabile di procedimento non ha il codice fiscale.", nameof(HasManagerFiscalCode))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            Contact fascManager = objectToValidate.Contacts?.SingleOrDefault();

            if (fascManager == null || string.IsNullOrEmpty(fascManager.FiscalCode))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}