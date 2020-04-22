using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Protocols
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsContainerActive : BaseValidator<Protocol, ProtocolValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsContainerActive(NameValueCollection attributes)
            : base("Il protocollo non ha il contenitore valido", nameof(IsContainerActive))
        {

        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ProtocolValidator objectToValidate)
        {
            Container container = null;
            if (objectToValidate.Container != null)
            {
                container = CurrentUnitOfWork.Repository<Container>().GetWithProtocolLocation(objectToValidate.Container.EntityShortId).FirstOrDefault();
            }
            if (container == null ||
                (container != null && string.IsNullOrEmpty(container.ProtLocation?.ProtocolArchive)))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
