using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleDocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsEnvironmentValid : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {


        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public IsEnvironmentValid(NameValueCollection attributes)
           : base("Il classificatore indicato non prevede un piano di fascicolazione e non è possibile fascicolare il documento, ma solo referenziare.", nameof(IsEnvironmentValid))
        {
        }
        #endregion

        #region [ Methods ]

        protected override void ValidateObject(FascicleDocumentUnitValidator objectToValidate)
        {
            if (objectToValidate.ReferenceType == ReferenceType.Reference)
            {
                return;
            }

            int count = 0;
            Fascicle localFascicle = objectToValidate.Fascicle == null || objectToValidate.DocumentUnit == null ? null : CurrentUnitOfWork.Repository<Fascicle>().GetByUniqueId(objectToValidate.Fascicle.UniqueId);

            count = localFascicle == null || localFascicle.Category == null ? 0 :
                CurrentUnitOfWork.Repository<CategoryFascicle>().CountByEnvironment(localFascicle.Category.EntityShortId, objectToValidate.DocumentUnit.Environment);

            if (count == 0)
            {
                GenerateInvalidateResult();
            }

        }
        #endregion
    }
}
