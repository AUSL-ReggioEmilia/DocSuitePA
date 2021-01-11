using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsAssociatedWithProcess : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsAssociatedWithProcess(NameValueCollection attributes)
            : base("Questa cartella di dossier non è associata a una serie valida.", nameof(IsAssociatedWithProcess))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            bool hasProcess = CurrentUnitOfWork.Repository<DossierFolder>().HasProcessAssociated(objectToValidate.Dossier.UniqueId);
            if (!hasProcess)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
