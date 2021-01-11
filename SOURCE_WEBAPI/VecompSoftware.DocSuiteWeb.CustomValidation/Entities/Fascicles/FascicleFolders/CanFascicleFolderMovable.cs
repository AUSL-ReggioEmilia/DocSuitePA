using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class CanFascicleFolderMovable : BaseValidator<FascicleFolder, FascicleFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public CanFascicleFolderMovable(NameValueCollection attributes)
            : base("la cartella di destinazione non può spostata in un figlio della cartella selezionata.", nameof(CanFascicleFolderMovable))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleFolderValidator objectToValidate)
        {
            FascicleFolder destinationfolder = CurrentUnitOfWork.Repository<FascicleFolder>().GetByFolderId(objectToValidate.ParentInsertId.Value);
            bool result = destinationfolder.FascicleFolderPath.StartsWith(objectToValidate.FascicleFolderPath);
            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
