using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;


namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasAccountedRole : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasAccountedRole(NameValueCollection attributes)
            : base("Nelle cartelle il settore indicato deve essere di tipo Accounted.", nameof(HasAccountedRole))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            if (objectToValidate.DossierFolderRoles != null && objectToValidate.DossierFolderRoles.Any() && objectToValidate.DossierFolderRoles.Any(x => x.AuthorizationRoleType != AuthorizationRoleType.Accounted))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
