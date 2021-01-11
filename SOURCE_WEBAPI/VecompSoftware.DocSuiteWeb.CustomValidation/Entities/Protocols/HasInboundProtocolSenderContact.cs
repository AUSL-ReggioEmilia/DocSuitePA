using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Protocols
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasInboundProtocolSenderContact : BaseValidator<Protocol, ProtocolValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasInboundProtocolSenderContact(NameValueCollection attributes)
            : base("Il protocollo non ha contatti dal mittente .", nameof(HasInboundProtocolSenderContact))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ProtocolValidator objectToValidate)
        {
            if (objectToValidate.ProtocolType != null && objectToValidate.ProtocolType.EntityShortId == (short)ProtocolTypology.Inbound &&
                objectToValidate.ProtocolContacts.Count(f => f.ComunicationType == "M") +
                objectToValidate.ProtocolContactManuals.Count(f => f.ComunicationType == "M") <= 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
