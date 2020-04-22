using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsAssociatedWithFascicle : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsAssociatedWithFascicle(NameValueCollection attributes)
            : base("Non è possibile associare un Fascicolo ad una cartella di un Dossier chiusa o che contiene sottocartelle.", nameof(IsAssociatedWithFascicle))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            if (objectToValidate.Status != DossierFolderStatus.InProgress)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
