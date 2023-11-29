using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class PasswordEncryptionKeyValidator : Validator
    {
        #region [ Fields ]               
        private const string MESSAGE_EXCEPTION = "La password non è impostata. Contattare assistenza per inserisci una password.";
        #endregion

        #region [ Properties ]
        protected override string DefaultMessageTemplate => MESSAGE_EXCEPTION;
        #endregion

        #region [ Constructor ]
        public PasswordEncryptionKeyValidator(string val) : base(string.Empty, val) { }

        public PasswordEncryptionKeyValidator(NameValueCollection attributes)
            : base(string.Empty, string.Empty)
        {
        }
        #endregion

        #region [ Methods ]
        public override void DoValidate(object objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (string.IsNullOrEmpty(WebApiConfiguration.PasswordEncryptionKey) || WebApiConfiguration.PasswordEncryptionKey.Length != 32)
            {
                throw new DSWException(MESSAGE_EXCEPTION, null, DSWExceptionCode.VA_CustomValidatorObject);
            }
        }
        #endregion   
    }
}
