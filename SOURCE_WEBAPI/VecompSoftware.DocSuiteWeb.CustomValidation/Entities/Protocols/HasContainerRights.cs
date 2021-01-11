using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Protocols
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasContainerRights : BaseValidator<Protocol, ProtocolValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Fields ]
        public HasContainerRights(NameValueCollection attributes)
            : base("L'utente non ha diritti di inserimento sul contenitore .", nameof(HasContainerRights))
        {

        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ProtocolValidator objectToValidate)
        {
            DomainUserModel currentUser = objectToValidate.CurrentSecurity?.GetCurrentUser();
            bool res = (currentUser == null || objectToValidate.Container == null) ? false : CurrentUnitOfWork.Repository<Container>().CountProtocolInsertRight(currentUser.Name, currentUser.Domain) > 0;

            if (!res)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
