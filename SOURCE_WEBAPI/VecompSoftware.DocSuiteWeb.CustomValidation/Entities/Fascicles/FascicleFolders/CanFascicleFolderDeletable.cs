using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class CanFascicleFolderDeletable : BaseValidator<FascicleFolder, FascicleFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public CanFascicleFolderDeletable(NameValueCollection attributes)
            : base("la cartella non può essere eliminata in quanto contiene elementi.", nameof(CanFascicleFolderDeletable))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleFolderValidator objectToValidate)
        {
            bool result = CurrentUnitOfWork.Repository<FascicleDocumentUnit>().HasFascicleDocumentUnit(objectToValidate.UniqueId) || CurrentUnitOfWork.Repository<FascicleDocument>().HasFascicleDocument(objectToValidate.UniqueId);
            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
