using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class NameAlreadyExists : BaseValidator<FascicleFolder, FascicleFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public NameAlreadyExists(NameValueCollection attributes)
            : base("Il nome delle cartelle di un fascicolo deve essere univoco per cartelle dello stesso livello.", nameof(NameAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleFolderValidator objectToValidate)
        {
            bool result = (string.IsNullOrEmpty(objectToValidate.Name) || objectToValidate.Fascicle == null) ? true : CurrentUnitOfWork.Repository<FascicleFolder>().NameAlreadyExists(objectToValidate.Name, objectToValidate.ParentInsertId, objectToValidate.Fascicle.UniqueId);

            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
