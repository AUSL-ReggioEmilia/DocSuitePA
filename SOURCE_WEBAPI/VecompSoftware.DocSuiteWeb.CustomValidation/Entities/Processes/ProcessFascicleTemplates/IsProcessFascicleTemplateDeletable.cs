using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Processes.ProcessFascicleTemplates
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsProcessFascicleTemplateDeletable : BaseValidator<ProcessFascicleTemplate, ProcessFascicleTemplateValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public IsProcessFascicleTemplateDeletable(NameValueCollection attributes)
            : base("Il processo non può essere eliminato.", nameof(IsProcessFascicleTemplateDeletable))
        {
        }

        #endregion

        #region [ Methods ]

        protected override void ValidateObject(ProcessFascicleTemplateValidator objectToValidate)
        {
            if (objectToValidate.EndDate <= DateTime.UtcNow)
            {
                GenerateInvalidateResult();
            }
        }

        #endregion
    }
}
