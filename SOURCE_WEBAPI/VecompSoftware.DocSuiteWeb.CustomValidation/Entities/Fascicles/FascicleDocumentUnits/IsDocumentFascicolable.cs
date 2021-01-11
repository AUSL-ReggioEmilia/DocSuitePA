using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.FascicleDocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsDocumentFascicolable : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {


        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsDocumentFascicolable(NameValueCollection attributes)
           : base("Il document selezionato è già stato fascicolato.", nameof(IsDocumentUnitFascicolable))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleDocumentUnitValidator objectToValidate)
        {
            IEnumerable<CategoryFascicle> temp = (objectToValidate.Fascicle == null || objectToValidate.Fascicle.Category == null) ? null :
                                                CurrentUnitOfWork.Repository<CategoryFascicle>().GetPeriodicByCategory(objectToValidate.Fascicle.Category.EntityShortId, Convert.ToInt32(DSWEnvironmentType.Protocol));

            if (temp != null && temp.Any() && objectToValidate.Fascicle.FascicleType == FascicleType.Procedure && objectToValidate.ReferenceType == ReferenceType.Fascicle)
            {
                GenerateInvalidateResult();
            }
        }

        #endregion

    }
}
