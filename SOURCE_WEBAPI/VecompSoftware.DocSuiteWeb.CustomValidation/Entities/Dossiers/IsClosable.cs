using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Dossiers
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsClosable : BaseValidator<Dossier, DossierValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsClosable(NameValueCollection attributes)
            : base("Solo se tutti i fascicoli sono chiusi il dossier può essere chiuso.", nameof(IsClosable))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DossierValidator objectToValidate)
        {
            bool result = objectToValidate.Status == DossierStatus.Closed && !CurrentUnitOfWork.Repository<Dossier>().AllFasciclesAreClosed(objectToValidate.UniqueId);

            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
