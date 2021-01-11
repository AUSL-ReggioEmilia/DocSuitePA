using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Protocols
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasProtocolMainDocument : BaseValidator<Protocol, ProtocolValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasProtocolMainDocument(NameValueCollection attributes)
            : base("Il protocollo non ha il documento principale .", nameof(HasProtocolMainDocument))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ProtocolValidator objectToValidate)
        {
            if (objectToValidate.IdDocument == null || objectToValidate.IdDocument <= 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
