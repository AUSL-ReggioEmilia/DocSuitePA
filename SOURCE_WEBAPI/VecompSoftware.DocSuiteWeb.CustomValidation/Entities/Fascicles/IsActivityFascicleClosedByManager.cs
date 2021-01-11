using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsActivityFascicleClosedByManager : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor]
        public IsActivityFascicleClosedByManager(NameValueCollection attributes)
            : base("L'utente non ha diritti per chiudere questo fascicolo.", nameof(IsActivityFascicleClosedByManager))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            DomainUserModel currentUser = objectToValidate.CurrentSecurity?.GetCurrentUser();

            bool result = currentUser == null ? false : CurrentUnitOfWork.Repository<Fascicle>().GetIfCurrentUserIsManagerOnActivityFascicle(currentUser.DisplayName, objectToValidate.UniqueId);

            if (!result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
