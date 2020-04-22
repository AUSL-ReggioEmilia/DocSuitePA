using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Resolutions.ResolutionKindDocumentSeries
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasAlreadyDocumentSeries : BaseValidator<Entity.Resolutions.ResolutionKindDocumentSeries, ResolutionKindDocumentSeriesValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasAlreadyDocumentSeries(NameValueCollection attributes)
            : base("Esiste già la medesima serie documentale nella tipologia di atto corrente.", nameof(HasAlreadyDocumentSeries))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ResolutionKindDocumentSeriesValidator objectToValidate)
        {
            bool result = true;
            if (objectToValidate.DocumentSeries != null && objectToValidate.ResolutionKind != null)
            {
                result = CurrentUnitOfWork.Repository<Entity.Resolutions.ResolutionKindDocumentSeries>().HasAlreadyDocumentSeries(objectToValidate.DocumentSeries.EntityId, objectToValidate.ResolutionKind.UniqueId, objectToValidate.UniqueId);
            }
            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
