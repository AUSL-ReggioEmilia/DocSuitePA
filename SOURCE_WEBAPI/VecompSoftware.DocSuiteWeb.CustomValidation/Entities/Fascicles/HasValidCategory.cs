using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasValidCategory : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidCategory(NameValueCollection attributes)
            : base("Sul classificatore selezionato non è stato definito nessun piano di fascicolazione.", nameof(HasValidCategory))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            IEnumerable<CategoryFascicle> temp = objectToValidate.Category == null ? null : CurrentUnitOfWork.Repository<CategoryFascicle>().GetByCategory(objectToValidate.Category.EntityShortId);

            if (temp == null || !temp.Any())
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}