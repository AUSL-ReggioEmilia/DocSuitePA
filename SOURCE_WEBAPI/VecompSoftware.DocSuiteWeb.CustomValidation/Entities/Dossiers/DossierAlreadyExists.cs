using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class DossierAlreadyExists : BaseValidator<Dossier, DossierValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public DossierAlreadyExists(NameValueCollection attributes)
            : base("La combinazione Anno-Numero del Dossier deve essere univoca.", nameof(DossierAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierValidator objectToValidate)
        {
            int res = CurrentUnitOfWork.Repository<Dossier>().CountDossierByNumbering(objectToValidate.Year, objectToValidate.Number);

            if (res > 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
