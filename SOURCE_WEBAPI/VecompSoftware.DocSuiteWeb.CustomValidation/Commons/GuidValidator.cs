using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class GuidValidator : Validator<Guid?>
    {
        #region [ Fields ]
        private const string MESSAGE_EXCEPTION = "The value is null or empty.";
        #endregion

        #region [ Properties ]
        protected override string DefaultMessageTemplate => MESSAGE_EXCEPTION;
        #endregion

        #region [ Constructor ]
        public GuidValidator(string val) : base(string.Empty, val) { }

        public GuidValidator(NameValueCollection attributes)
            : base(string.Empty, string.Empty)
        {
        }
        #endregion

        #region [ Methods ]
        protected override void DoValidate(Guid? objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate == null)
            {
                throw new DSWException(MESSAGE_EXCEPTION, null, DSWExceptionCode.VA_CustomValidatorObject);
            }

            if (!objectToValidate.HasValue || objectToValidate.Value == Guid.Empty)
            {
                LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
            }
        }
        #endregion        
    }
}
