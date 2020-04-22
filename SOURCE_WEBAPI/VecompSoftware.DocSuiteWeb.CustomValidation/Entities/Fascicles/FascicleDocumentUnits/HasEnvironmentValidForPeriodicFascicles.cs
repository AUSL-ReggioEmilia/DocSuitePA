using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleDocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasEnvironmentValidForPeriodicFascicles : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasEnvironmentValidForPeriodicFascicles(NameValueCollection nameValueCollection)
            : base("L'environment dell'unità documentaria scelta non corrisponde a quello del fascicolo periodico .", nameof(HasEnvironmentValidForPeriodicFascicles))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleDocumentUnitValidator objectToValidate)
        {
            Fascicle localFascicle = objectToValidate.Fascicle == null ? null : CurrentUnitOfWork.Repository<Fascicle>().GetByUniqueId(objectToValidate.Fascicle.UniqueId);
            DocumentUnit documentUnit = objectToValidate.DocumentUnit == null ? null : CurrentUnitOfWork.Repository<DocumentUnit>().GetById(objectToValidate.DocumentUnit.UniqueId).SingleOrDefault();

            if (documentUnit != null && localFascicle != null && !((documentUnit.Environment).Equals(localFascicle.DSWEnvironment)) && objectToValidate.ReferenceType == ReferenceType.Fascicle && localFascicle.FascicleType == FascicleType.Period)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
