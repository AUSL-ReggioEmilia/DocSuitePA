using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasAlreadyPeriodicFascicle : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasAlreadyPeriodicFascicle(NameValueCollection attributes)
            : base("Sul nodo di classificazione indicato è già presente almeno un fascicolo periodico attivo.", nameof(HasAlreadyPeriodicFascicle))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            if (objectToValidate.FascicleType != FascicleType.Period)
            {
                return;
            }

            IEnumerable<Fascicle> temp = objectToValidate.Category == null ? null : CurrentUnitOfWork.Repository<Fascicle>().GetActiveFascicles(objectToValidate.Category.EntityShortId);

            if (temp == null || (temp != null && temp.Any() && temp.Any(f => f.FascicleType == FascicleType.Period)))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}