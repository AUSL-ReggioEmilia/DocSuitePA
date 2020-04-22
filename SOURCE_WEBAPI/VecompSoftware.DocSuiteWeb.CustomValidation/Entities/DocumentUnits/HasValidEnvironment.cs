using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.DocumentUnits
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasValidEnvironment : BaseValidator<DocumentUnit, DocumentUnitValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidEnvironment(NameValueCollection attributes)
            : base("L'Environment della DocumentUnit indicata risulta non essere valido.", nameof(HasValidEnvironment))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(DocumentUnitValidator objectToValidate)
        {
            IEnumerable<int> environments = ((int[])Enum.GetValues(typeof(DSWEnvironmentType))).Where(f => f != 0);
            IEnumerable<int> udsEnvironments = CurrentUnitOfWork.Repository<UDSRepository>().GetEnvironments();

            environments = environments.Concat(udsEnvironments).ToList();
            if (!environments.Contains(objectToValidate.Environment))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
