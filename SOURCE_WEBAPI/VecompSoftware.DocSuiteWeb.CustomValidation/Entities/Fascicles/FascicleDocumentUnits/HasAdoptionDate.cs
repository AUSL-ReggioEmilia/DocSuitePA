using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleDocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasAdoptionDate : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasAdoptionDate(NameValueCollection attributes)
            : base("Non è possibile inserire l'atto nel fascicolo selezionato perchè l'atto non è ancora stato adottato.", nameof(HasAdoptionDate))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleDocumentUnitValidator objectToValidate)
        {
            if (objectToValidate.DocumentUnit.Environment != (int)DSWEnvironmentType.Resolution)
            {
                return;
            }

            IQueryable<Resolution> res = objectToValidate.DocumentUnit == null ? null : CurrentUnitOfWork.Repository<Resolution>().GetByUniqueId(objectToValidate.DocumentUnit.UniqueId);
            Resolution resolution = res?.SingleOrDefault();

            if (resolution == null || !resolution.AdoptionDate.HasValue)
            {
                GenerateInvalidateResult();
            }
            #endregion
        }
    }
}