using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsFascicleClosed : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsFascicleClosed(NameValueCollection attributes)
            : base("Il Fascicolo corrente risulta già chiuso.", nameof(IsFascicleClosed))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            Fascicle fascicle = CurrentUnitOfWork.Repository<Fascicle>().GetByUniqueId(objectToValidate.UniqueId);

            if (fascicle == null || fascicle.EndDate.HasValue)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
