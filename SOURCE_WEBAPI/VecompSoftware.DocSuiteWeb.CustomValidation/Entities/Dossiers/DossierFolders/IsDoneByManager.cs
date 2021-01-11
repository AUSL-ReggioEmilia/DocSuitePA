using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsDoneByManager : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor]
        public IsDoneByManager(NameValueCollection attributes)
            : base("L'utente non ha diritti per creare/modificare/eliminare una cartella di questo Dossier.", nameof(IsDoneByManager))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            DomainUserModel currentUser = objectToValidate.CurrentSecurity?.GetCurrentUser();

            bool result = (currentUser == null || objectToValidate.Dossier == null) ? false : CurrentUnitOfWork.Repository<Dossier>().HasDossierManageable(objectToValidate.Dossier.UniqueId, currentUser.Name, currentUser.Domain);

            if (!result)
            {
                GenerateInvalidateResult();
            }

        }
        #endregion
    }
}
