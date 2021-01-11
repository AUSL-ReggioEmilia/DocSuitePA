using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Resolutions.ResolutionKinds
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsDeletable : BaseValidator<ResolutionKind, ResolutionKindValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsDeletable(NameValueCollection attributes)
            : base("Cancellazione tipologia di atto non consentita: Esiste almeno un atto associato.", nameof(IsDeletable))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ResolutionKindValidator objectToValidate)
        {
            bool result = CurrentUnitOfWork.Repository<Resolution>().CountByResolutionKind(objectToValidate.UniqueId) > 0;
            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
