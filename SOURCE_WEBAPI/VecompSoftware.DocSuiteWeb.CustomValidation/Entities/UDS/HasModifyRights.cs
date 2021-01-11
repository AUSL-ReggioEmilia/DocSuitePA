using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.UDS
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasModifyRights : BaseValidator<UDSDocumentUnit, UDSDocumentUnitValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasModifyRights(NameValueCollection attributes)
            : base("L'utente non ha diritti di Modifica sul archivi.", nameof(HasModifyRights))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(UDSDocumentUnitValidator objectToValidate)
        {

            DomainUserModel currentUser = objectToValidate.CurrentSecurity?.GetCurrentUser();
            bool res = (currentUser == null || objectToValidate.Repository.Container == null) ? false : CurrentUnitOfWork.Repository<Container>().HasUDSDocumentUnitModifyRight(currentUser.Name, currentUser.Domain, objectToValidate.Repository.Container.EntityShortId);
            if (!res)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
