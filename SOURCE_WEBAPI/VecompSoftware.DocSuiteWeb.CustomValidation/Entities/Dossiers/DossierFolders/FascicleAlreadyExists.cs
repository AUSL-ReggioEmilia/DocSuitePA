using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers.DossierFolders
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class FascicleAlreadyExists : BaseValidator<DossierFolder, DossierFolderValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public FascicleAlreadyExists(NameValueCollection attributes)
            : base("Esiste già il fascicolo selezionato in altre cartelle del livello selezionato.", nameof(FascicleAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierFolderValidator objectToValidate)
        {
            bool result = (objectToValidate.Dossier == null || objectToValidate.Fascicle == null) ? true : CurrentUnitOfWork.Repository<DossierFolder>().FascicleAlreadyExists(objectToValidate.Fascicle.UniqueId, objectToValidate.ParentInsertId, objectToValidate.Dossier.UniqueId);

            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
