using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class MinDateTimeOffsetValidator : Validator<DateTimeOffset>
    {
        #region [ Fields ]   
        private const string MESSAGE_RESULT = "La data non può essere nulla.";
        private const string MESSAGE_EXCEPTION = "The MinDateTimeOffsetValidator is null";
        #endregion

        public MinDateTimeOffsetValidator(NameValueCollection attributes)
      : base(string.Empty, string.Empty)
        {
        }

        protected override string DefaultMessageTemplate => MESSAGE_RESULT;

        protected override void DoValidate(DateTimeOffset objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate == null)
            {
                throw new DSWException(MESSAGE_EXCEPTION, null, DSWExceptionCode.VA_CustomValidatorObject);
            }

            if (objectToValidate == DateTimeOffset.MinValue)
            {
                LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
            }
        }
    }
}
