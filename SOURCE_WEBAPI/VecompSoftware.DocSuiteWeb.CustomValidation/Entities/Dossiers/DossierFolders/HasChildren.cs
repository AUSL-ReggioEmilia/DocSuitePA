using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasChildren : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasChildren(NameValueCollection attributes)
            : base("Non è possibile eliminare una cartella di un Dossier che ha sottocartelle.", nameof(HasChildren))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            bool result = objectToValidate.Status == DossierFolderStatus.Folder || CurrentUnitOfWork.Repository<DossierFolder>().CountChildren(objectToValidate.UniqueId) > 0;
            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion

    }
}
