using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class NameAlreadyExists : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public NameAlreadyExists(NameValueCollection attributes)
            : base("Il nome delle cartelle di un Dossier deve essere univoco per cartelle dello stesso livello.", nameof(NameAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            bool result = (string.IsNullOrEmpty(objectToValidate.Name) || objectToValidate.Dossier == null) ? true : CurrentUnitOfWork.Repository<DossierFolder>().NameAlreadyExists(objectToValidate.Name, objectToValidate.ParentInsertId, objectToValidate.Dossier.UniqueId);

            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
