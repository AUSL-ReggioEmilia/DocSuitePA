using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsPeriodicFascicleUnique : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsPeriodicFascicleUnique(NameValueCollection attributes)
            : base("Il fascicolo periodico aperto deve essere unico per ciascuna tipologia documentale e classificazione.", nameof(IsPeriodicFascicleUnique))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            bool res = objectToValidate.Category == null ? true :
                CurrentUnitOfWork.Repository<Fascicle>().ExistActivePeriodic(objectToValidate.Category.EntityShortId, objectToValidate.DSWEnvironment == null ? 0 : objectToValidate.DSWEnvironment.Value, objectToValidate.Container?.EntityShortId);

            if (res)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}