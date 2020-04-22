using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Conservations;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsNotConservated : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsNotConservated(NameValueCollection attributes)
            : base("Non è possibile aprire un fascicolo già conservato.", nameof(IsNotConservated))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            Conservation conservation = CurrentUnitOfWork.Repository<Conservation>().GetByIdWithStatus(objectToValidate.UniqueId, ConservationStatus.Conservated).FirstOrDefault();

            if (conservation != null)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
