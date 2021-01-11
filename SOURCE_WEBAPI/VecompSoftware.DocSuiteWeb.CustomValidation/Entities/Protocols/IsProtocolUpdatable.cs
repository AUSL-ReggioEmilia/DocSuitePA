using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Protocols
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsProtocolUpdatable : BaseValidator<Protocol, ProtocolValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsProtocolUpdatable(NameValueCollection attributes)
            : base("Il protocollo non è aggiornabile .", nameof(IsProtocolUpdatable))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ProtocolValidator objectToValidate)
        {
            Protocol protocol = CurrentUnitOfWork.Repository<Entity.Protocols.Protocol>().GetByUniqueId(objectToValidate.UniqueId).FirstOrDefault();

            if (protocol == null)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
