using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.UDS
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasUDSRepository : BaseValidator<UDSFieldList, UDSFieldListValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasUDSRepository(NameValueCollection attributes)
            : base("Il repository non esiste.", nameof(IsActive))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(UDSFieldListValidator objectToValidate)
        {
            if (objectToValidate.Repository == null)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
