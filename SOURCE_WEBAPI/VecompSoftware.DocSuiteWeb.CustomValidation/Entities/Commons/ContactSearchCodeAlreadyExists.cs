using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class ContactSearchCodeAlreadyExists : BaseValidator<Contact, ContactValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ] 
        public ContactSearchCodeAlreadyExists(NameValueCollection attributes)
            : base("Esiste già un contatto con il codice di ricerca impostato.", nameof(ContactSearchCodeAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ContactValidator objectToValidate)
        {
            if (!string.IsNullOrEmpty(objectToValidate?.SearchCode))
            {
                int count = CurrentUnitOfWork.Repository<Contact>().CountContactBySearchCode(objectToValidate.SearchCode);
                if (count > 0)
                {
                    GenerateInvalidateResult();
                }
            }            
        }
        #endregion
    }
}
