using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasAssociatedFolder : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasAssociatedFolder(NameValueCollection attributes)
            : base("Il fascicolo non si può chiudere, sono ancora presenti dei procedimenti aperti associati al fascicolo.", nameof(HasAssociatedFolder))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            if (objectToValidate.EndDate.HasValue && objectToValidate.DossierFolders != null && objectToValidate.DossierFolders.Any()
                && objectToValidate.DossierFolders.Any(f => f.Status != DossierFolderStatus.FascicleClose))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
