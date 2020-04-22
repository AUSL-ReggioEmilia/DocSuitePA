using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleDocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    internal class IsDocumentUnitFascicolable : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {


        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsDocumentUnitFascicolable(NameValueCollection attributes)
           : base("Il document unit selezionato è già stato fascicolato.", nameof(IsDocumentUnitFascicolable))
        {
        }

        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleDocumentUnitValidator objectToValidate)
        {
            if (objectToValidate.ReferenceType != ReferenceType.Fascicle)
            {
                return;
            }

            int count = 0;

            count = objectToValidate.DocumentUnit == null ? 1 : CurrentUnitOfWork.Repository<FascicleDocumentUnit>().CountByReferenceTypeAndDocumentUnit(objectToValidate.DocumentUnit.UniqueId, ReferenceType.Fascicle);

            if (count > 0)
            {
                GenerateInvalidateResult();
            }
        }

        #endregion


    }
}
