using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class ActivityFascicleAlreadyExists : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public ActivityFascicleAlreadyExists(NameValueCollection attributes)
            : base("La combinazione Anno-Numero del Fascicolo deve essere univoca.", nameof(ActivityFascicleAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            int res = CurrentUnitOfWork.Repository<Fascicle>().CountActivityFascicleByNumbering(objectToValidate.Year, objectToValidate.Number);

            if (res > 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
