using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;


namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasResponsibleRole : BaseValidator<Dossier, DossierValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasResponsibleRole(NameValueCollection attributes)
            : base("Nel Dossier deve essere indicato almeno un settore Responsible .", nameof(HasResponsibleRole))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierValidator objectToValidate)
        {
            if (objectToValidate.DossierRoles == null || !objectToValidate.DossierRoles.Any() || objectToValidate.DossierRoles.Any(x => x.AuthorizationRoleType != AuthorizationRoleType.Responsible))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
