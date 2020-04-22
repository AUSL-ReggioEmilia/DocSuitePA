using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Protocols
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsProtocolCreatable : BaseValidator<Protocol, ProtocolValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsProtocolCreatable(NameValueCollection attributes)
            : base("Il protocollo esiste già", nameof(IsProtocolCreatable))
        {

        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ProtocolValidator objectToValidate)
        {
            int count = objectToValidate == null ? 0 : CurrentUnitOfWork.Repository<Protocol>().CountByUniqueId(objectToValidate.UniqueId);

            if (count != 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
