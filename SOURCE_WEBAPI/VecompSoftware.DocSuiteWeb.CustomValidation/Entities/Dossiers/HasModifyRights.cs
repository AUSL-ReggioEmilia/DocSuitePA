using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;


namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasModifyRights : BaseValidator<Dossier, DossierValidator>
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
        protected override void ValidateObject(DossierValidator objectToValidate)
        {
            DomainUserModel currentUser = objectToValidate.CurrentSecurity?.GetCurrentUser();

            bool res = (currentUser == null || objectToValidate.Container == null) ? false : CurrentUnitOfWork.Repository<Container>().CountDossierModifyRight(currentUser.Name, currentUser.Domain, objectToValidate.Container.EntityShortId) > 0;

            if (!res)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
