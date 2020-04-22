using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Resolutions.ResolutionKinds
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class NameAlreadyExists : BaseValidator<ResolutionKind, ResolutionKindValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public NameAlreadyExists(NameValueCollection attributes)
            : base("Il nome della tipologia di atto deve essere univoco.", nameof(NameAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ResolutionKindValidator objectToValidate)
        {
            bool result = true;
            if (!string.IsNullOrEmpty(objectToValidate.Name))
            {
                result = CurrentUnitOfWork.Repository<ResolutionKind>().NameAlreadyExists(objectToValidate.Name, objectToValidate.UniqueId);
            }
            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
