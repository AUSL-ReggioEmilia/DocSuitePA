using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Finder.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.DocumentArchives
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class DocumentSeriesConstraintAlreadyExists : BaseValidator<DocumentSeriesConstraint, DocumentSeriesConstraintValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public DocumentSeriesConstraintAlreadyExists(NameValueCollection attributes)
            : base("E' già presente un obbligo di trasparenza con lo stesso nome per la serie specificata.", nameof(DocumentSeriesConstraintAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]

        protected override void ValidateObject(DocumentSeriesConstraintValidator objectToValidate)
        {
            int res = CurrentUnitOfWork.Repository<DocumentSeriesConstraint>().CountExistingConstraint(objectToValidate.Name, objectToValidate.DocumentSeries.EntityId, objectToValidate.UniqueId);
            if (res > 0)
            {
                GenerateInvalidateResult();
            }

        }
        #endregion
    }
}
