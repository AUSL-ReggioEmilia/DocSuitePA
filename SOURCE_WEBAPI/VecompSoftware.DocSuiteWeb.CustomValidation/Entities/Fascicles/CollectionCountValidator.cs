using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Collections;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.Configuration;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CollectionCountValidatorData))]
    public class CollectionCountValidator : Validator<IEnumerable>
    {
        public CollectionCountValidator(string tag) : base(string.Empty, tag) { }

        public CollectionCountValidator(NameValueCollection attributes)
            : base(string.Empty, string.Empty)
        {
        }

        protected override string DefaultMessageTemplate => $"Count validator '{GetType().Name}' is null";

        protected override void DoValidate(IEnumerable objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            foreach (object item in objectToValidate)
            {
                return;
            }
            LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);

        }
    }
}