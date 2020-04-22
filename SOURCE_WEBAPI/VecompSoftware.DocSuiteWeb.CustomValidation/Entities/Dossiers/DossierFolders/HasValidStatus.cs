using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasValidStatus : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidStatus(NameValueCollection attributes)
            : base("Non è possibile inserire una cartella di un Dossier con questo tipo di Status.", nameof(HasValidStatus))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            if ((objectToValidate.Fascicle != null && objectToValidate.Status != DossierFolderStatus.Fascicle) || (objectToValidate.Fascicle == null && objectToValidate.Status != DossierFolderStatus.InProgress))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
