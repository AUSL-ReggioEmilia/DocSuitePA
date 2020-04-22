using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Specialized;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class TrueValidator : Validator<bool>
    {

        public TrueValidator(string tag) : base(string.Empty, tag) { }

        public TrueValidator(NameValueCollection attributes)
            : base(string.Empty, string.Empty)
        {
        }

        protected override string DefaultMessageTemplate => "Value is true";

        protected override void DoValidate(bool objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate == true)
            {
                LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
            }
        }


    }
}
