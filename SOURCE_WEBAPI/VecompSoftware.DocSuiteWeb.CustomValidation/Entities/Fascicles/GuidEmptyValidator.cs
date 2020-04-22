using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.Configuration;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(GuidEmptyValidatorData))]
    public class GuidEmptyValidator : Validator<Guid>
    {

        private const string MESSAGE_RESULT = "Non è presente un archivio valido";

        public GuidEmptyValidator(string tag) : base(string.Empty, tag) { }

        public GuidEmptyValidator(NameValueCollection attributes)
            : base(string.Empty, string.Empty)
        {
        }

        protected override string DefaultMessageTemplate => MESSAGE_RESULT;

        protected override void DoValidate(Guid objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate == Guid.Empty)
            {
                LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
            }
        }
    }
}