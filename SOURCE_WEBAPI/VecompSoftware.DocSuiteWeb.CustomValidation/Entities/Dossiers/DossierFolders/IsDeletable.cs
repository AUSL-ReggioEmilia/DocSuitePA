using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsDeletable : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsDeletable(NameValueCollection attributes)
            : base("Non è possibile eliminare una cartella di un Dossier che ha un fascicolo associato.", nameof(IsDeletable))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            if (objectToValidate.Fascicle != null || objectToValidate.Status == DossierFolderStatus.Fascicle || objectToValidate.Status == DossierFolderStatus.FascicleClose)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
