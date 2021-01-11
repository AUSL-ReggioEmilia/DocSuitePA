using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasParentValidStatus : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasParentValidStatus(NameValueCollection attributes)
            : base("Non è possibile inserire una sottocartella in una cartella associata ad un Fascicolo.", nameof(HasParentValidStatus))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            bool result = CurrentUnitOfWork.Repository<DossierFolder>().IsParentFascicleFolder(objectToValidate.ParentInsertId) > 0 ? false : true;

            bool hasParentValidStatus = objectToValidate.ParentInsertId == null ? true : result;

            if (!hasParentValidStatus)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
