using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsResponsibleRoleUnique : BaseValidator<Dossier, DossierValidator>
    {

        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsResponsibleRoleUnique(NameValueCollection attributes)
            : base("Il settore Responsible deve essere unico.", nameof(IsResponsibleRoleUnique))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierValidator objectToValidate)
        {
            if (objectToValidate.DossierRoles == null || !objectToValidate.DossierRoles.Any() || objectToValidate.DossierRoles.Count > 1)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}