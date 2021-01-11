using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class FascicleAlreadyExists : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public FascicleAlreadyExists(NameValueCollection attributes)
            : base("La combinazione Anno-Numero-Classificatore del Fascicolo deve essere univoca.", nameof(FascicleAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            int res = 0;

            res = objectToValidate.Category == null ? 1 :
                CurrentUnitOfWork.Repository<Fascicle>().CountByNumbering(objectToValidate.Year, objectToValidate.Number, objectToValidate.Category.EntityShortId);

            if (res > 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}