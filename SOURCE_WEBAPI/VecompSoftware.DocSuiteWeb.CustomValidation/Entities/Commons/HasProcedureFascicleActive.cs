using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasProcedureFascicleActive : BaseValidator<CategoryFascicle, CategoryFascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ] 
        public HasProcedureFascicleActive(NameValueCollection attributes)
            : base("Non è possibile eliminare un piano di procedimento se esistono fascicoli aperti.", nameof(HasProcedureFascicleActive))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(CategoryFascicleValidator objectToValidate)
        {
            if (objectToValidate.FascicleType != FascicleType.Procedure)
            {
                return;
            }

            bool validated = false;
            if (objectToValidate.Category != null)
            {
                validated = CurrentUnitOfWork.Repository<Fascicle>().GetAvailableProcedureFasciclesByCategory(objectToValidate.Category.EntityShortId).Count() == 0;
            }
            if (!validated)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
