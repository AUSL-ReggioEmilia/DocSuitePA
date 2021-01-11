using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Processes
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsProcessDeletable : BaseValidator<Process, ProcessValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public IsProcessDeletable(NameValueCollection attributes)
            : base("La serie non può essere eliminata.", nameof(IsProcessDeletable))
        {
        }

        #endregion

        #region [ Methods ]

        protected override void ValidateObject(ProcessValidator objectToValidate)
        {
            if (objectToValidate.EndDate > DateTime.UtcNow)
            {
                GenerateInvalidateResult();
            }
        }

        #endregion
    }
}
