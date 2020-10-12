using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierRoles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasModifyRights : BaseValidator<DossierRole, DossierRoleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasModifyRights(NameValueCollection attributes)
            : base("L'utente non ha diritti di Modifica sul contenitore.", nameof(HasModifyRights))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierRoleValidator objectToValidate)
        {
            DomainUserModel currentUser = objectToValidate.CurrentSecurity?.GetCurrentUser();

            bool res = (currentUser == null || objectToValidate.Dossier == null) ? false : CurrentUnitOfWork.Repository<Dossier>().HasDossierModifyRight(currentUser.Name, currentUser.Domain, objectToValidate.Dossier.UniqueId);

            if (!res)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
