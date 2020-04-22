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
    public class HasValidFascicleType : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidFascicleType(NameValueCollection attributes)
            : base("La tipologia di fascicolo passata non corrisponde a quella definita dal piano di fascicolazione.", nameof(HasValidFascicleType))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            IEnumerable<CategoryFascicle> temp = objectToValidate.Category == null ? null :
                CurrentUnitOfWork.Repository<CategoryFascicle>().GetByFascicleType(objectToValidate.Category.EntityShortId, objectToValidate.FascicleType);

            if (temp == null || !temp.Any())
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}