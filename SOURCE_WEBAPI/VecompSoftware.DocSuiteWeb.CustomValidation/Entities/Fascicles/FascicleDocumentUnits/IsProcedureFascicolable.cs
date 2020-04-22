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
    public class IsProcedureFascicolable : BaseValidator<FascicleDocumentUnit, FascicleDocumentUnitValidator>
    {


        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsProcedureFascicolable(NameValueCollection attributes)
            : base("L'unità documentaria non può essere fascicolata su un fascicolo di procedimento, quando sul classificatore è definito un piano di fascicolazione periodico", nameof(IsProcedureFascicolable))
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
