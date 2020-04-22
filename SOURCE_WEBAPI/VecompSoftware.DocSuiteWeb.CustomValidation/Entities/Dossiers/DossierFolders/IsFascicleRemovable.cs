using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsFascicleRemovable : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsFascicleRemovable(NameValueCollection attributes)
            : base("Non è possibile rimuovere un Fascicolo chiusa alla cartella del Dossier.", nameof(IsFascicleRemovable))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            if (objectToValidate.Fascicle == null || objectToValidate.Status == DossierFolderStatus.FascicleClose || objectToValidate.Fascicle.EndDate.HasValue)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
