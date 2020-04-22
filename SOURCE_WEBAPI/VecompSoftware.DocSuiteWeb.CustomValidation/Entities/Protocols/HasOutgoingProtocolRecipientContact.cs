using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Protocols
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasOutgoingProtocolRecipientContact : BaseValidator<Protocol, ProtocolValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasOutgoingProtocolRecipientContact(NameValueCollection attributes)
            : base("Il protocollo non ha contatti destinatari .", nameof(HasOutgoingProtocolRecipientContact))
        {

        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ProtocolValidator objectToValidate)
        {
            if (objectToValidate.ProtocolType != null && objectToValidate.ProtocolType.EntityShortId == (short)ProtocolTypology.Outgoing &&
                objectToValidate.ProtocolContacts.Count(f => f.ComunicationType == "D") +
                objectToValidate.ProtocolContactManuals.Count(f => f.ComunicationType == "D") <= 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
