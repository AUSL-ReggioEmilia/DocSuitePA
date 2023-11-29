using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasPolicyRightOverridable : Validator
    {
        #region [ Fields ]
        private const string MESSAGE_EXCEPTION = "L'utente non ha diritti di gestione.";
        private const string CURRENT_SECURITY_PROPERTY_NAME = "CurrentSecurity";
        private const string PARAMETER_ENV_SERVICE_PROPERTY_NAME = "ParameterEnvService";
        #endregion

        #region [ Constructor ]
        public HasPolicyRightOverridable(string tag) : base(string.Empty, tag) { }

        public HasPolicyRightOverridable(NameValueCollection attributes)
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
            IDecryptedParameterEnvService parameterEnvService = (IDecryptedParameterEnvService)objectToValidate.GetType().GetProperty(PARAMETER_ENV_SERVICE_PROPERTY_NAME).GetValue(objectToValidate, null);
            ISecurity currentSecurity = (ISecurity)objectToValidate.GetType().GetProperty(CURRENT_SECURITY_PROPERTY_NAME).GetValue(objectToValidate, null);
            string currentUser = currentSecurity.GetCurrentUser().Account;
            if (!parameterEnvService.DocSuiteServiceAccounts.Any(x => x.Equals(currentUser, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
            }            
        }
        #endregion        
    }
}
