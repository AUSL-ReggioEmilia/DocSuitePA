using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class CategoryFascicleContainsPeriodic : BaseValidator<CategoryFascicle, CategoryFascicleValidator>
    {

        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public CategoryFascicleContainsPeriodic(NameValueCollection attributes)
            : base("Un piano di classificazione periodico non può essere svuotato se contiene fascicoli periodici attivi.", nameof(CategoryFascicleContainsPeriodic))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(CategoryFascicleValidator objectToValidate)
        {
            bool res = objectToValidate.FascicleType != FascicleType.Period
                ? false
                : CurrentUnitOfWork.Repository<Fascicle>().AnyActivePeriodicByCategoryAndEnvironment(objectToValidate.Category.EntityShortId, objectToValidate.DSWEnvironment);

            if (res)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion

    }
}
