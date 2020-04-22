using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsValidFolderLevel : BaseValidator<FascicleFolder, FascicleFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsValidFolderLevel(NameValueCollection attributes)
            : base("Non è possibile eliminare le cartelle del fascicolo gestite autonomamente dal sistema .", nameof(IsValidFolderLevel))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleFolderValidator objectToValidate)
        {
            if (objectToValidate.FascicleFolderLevel <= 2 || objectToValidate.Typology == FascicleFolderTypology.Root)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
