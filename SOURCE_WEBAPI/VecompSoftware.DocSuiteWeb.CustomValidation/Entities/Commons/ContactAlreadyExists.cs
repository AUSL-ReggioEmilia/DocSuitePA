using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class ContactAlreadyExists : BaseValidator<Contact, ContactValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ] 
        public ContactAlreadyExists(NameValueCollection attributes)
            : base("Il contatto che si stà cercando di inserire esiste già in rubrica.", nameof(ContactAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ContactValidator objectToValidate)
        {
            bool existContacts = CurrentUnitOfWork.Repository<Contact>().FindContactsByDescriptionOrFiscalCode(objectToValidate.Description, objectToValidate.FiscalCode, 
                objectToValidate.IncrementalFather, true).Any();
            if (existContacts)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
