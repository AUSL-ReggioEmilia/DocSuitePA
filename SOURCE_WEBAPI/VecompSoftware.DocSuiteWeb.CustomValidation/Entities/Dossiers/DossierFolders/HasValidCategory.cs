using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasValidCategory : BaseValidator<DossierFolder, DossierFolderValidator>
    {

        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidCategory(NameValueCollection attributes)
            : base("Il classificatore della cartella di un Dossier deve essere uguale al Classificatore del fascicolo ad essa associato.", nameof(HasValidCategory))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            Fascicle temp = objectToValidate.Fascicle == null ? null : CurrentUnitOfWork.Repository<Fascicle>().GetByUniqueId(objectToValidate.Fascicle.UniqueId);

            if (temp != null && temp.Category != null && objectToValidate.Category != null && objectToValidate.Category.UniqueId != temp.Category.UniqueId)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}