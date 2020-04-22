using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;


namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasCategoryWithFascicle : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasCategoryWithFascicle(NameValueCollection attributes)
            : base("In una cartella di un Dossier con fascicolo associato, deve essere indicato anche un Classificatore.", nameof(HasCategoryWithFascicle))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            if (objectToValidate.Fascicle != null && objectToValidate.Category == null)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
