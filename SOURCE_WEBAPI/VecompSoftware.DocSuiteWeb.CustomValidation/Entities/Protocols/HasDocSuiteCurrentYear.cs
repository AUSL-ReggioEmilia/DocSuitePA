using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Parameters;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Protocols
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasDocSuiteCurrentYear : BaseValidator<Protocol, ProtocolValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasDocSuiteCurrentYear(NameValueCollection attributes)
            : base("Non è possibile inserire protocolli senza aver prima efftuato la procedura di cambio anno .", nameof(HasDocSuiteCurrentYear))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ProtocolValidator objectToValidate)
        {
            Parameter parameter = CurrentUnitOfWork.Repository<Parameter>().GetParameters().First();

            if (parameter != null && parameter.LastUsedYear != (short)DateTime.Now.Year)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
