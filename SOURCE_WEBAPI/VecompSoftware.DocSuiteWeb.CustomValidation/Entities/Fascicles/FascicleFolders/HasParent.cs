using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasParent : BaseValidator<FascicleFolder, FascicleFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasParent(NameValueCollection attributes)
            : base("Non è possibile creare una cartella di un fascicolo che non ha un padre.", nameof(HasParent))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleFolderValidator objectToValidate)
        {
            bool result = CurrentUnitOfWork.Repository<FascicleFolder>().HasParent(objectToValidate.ParentInsertId, objectToValidate.Fascicle.UniqueId);
            if (objectToValidate.ParentInsertId == null || !result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
