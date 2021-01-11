using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsFascicleClosable : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsFascicleClosable(NameValueCollection attributes)
            : base("Solo il responsabile e le segreterie di procedimento possono chiudere il fascicolo.", nameof(IsFascicleClosable))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            if (objectToValidate.FascicleType == Entity.Fascicles.FascicleType.Legacy)
            {
                return;
            }

            Contact temp = objectToValidate.Contacts?.FirstOrDefault();
            DomainUserModel currentUser = objectToValidate.CurrentSecurity?.GetCurrentUser();

            bool result = (currentUser == null || temp == null || objectToValidate.Category == null) ? true : (!(temp.EmailAddress == objectToValidate.CurrentSecurity.CurrentUserEmail || temp.CertifiedMail == objectToValidate.CurrentSecurity.CurrentUserEmail)) &&
                                                                         (!CurrentUnitOfWork.Repository<RoleUser>().IsProcedureSecretary(currentUser.Name, currentUser.Domain, objectToValidate.Category.EntityShortId));
            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
