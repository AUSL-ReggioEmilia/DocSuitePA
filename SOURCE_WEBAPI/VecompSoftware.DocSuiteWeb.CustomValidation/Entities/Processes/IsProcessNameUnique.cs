using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Finder.Processes;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Processes
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsProcessNameUnique : BaseValidator<Process, ProcessValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public IsProcessNameUnique(NameValueCollection attributes)
            : base("La serie con il nome scelto è già stata creata nella stessa classificazione.", nameof(IsProcessNameUnique))
        {
        }

        #endregion

        #region [ Methods ]

        protected override void ValidateObject(ProcessValidator objectToValidate)
        {

            bool result = CurrentUnitOfWork.Repository<Process>().NameAlreadyExists(objectToValidate.Name, objectToValidate.UniqueId, objectToValidate.Category.UniqueId);
            if (result)
            {
                GenerateInvalidateResult();
            }
        }

        #endregion
    }
}
