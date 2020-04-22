using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class NotAllowedValidator : Validator
    {
        #region [ Fields ]
        private const string MESSAGE_EXCEPTION = "Operation is not allowed.";
        #endregion

        #region [ Constructor ]
        public NotAllowedValidator(string tag) : base(string.Empty, tag) { }

        public NotAllowedValidator(NameValueCollection attributes)
            : base(string.Empty, string.Empty)
        {
        }
        #endregion

        #region [ Properties ]
        protected override string DefaultMessageTemplate => MESSAGE_EXCEPTION;
        #endregion

        #region [ Methods ]
        public override void DoValidate(object objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
        }
        #endregion        
    }
}
