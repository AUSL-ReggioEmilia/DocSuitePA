﻿using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Processes.ProcessFascicleTemplates
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsValidDatePeriod : BaseValidator<ProcessFascicleTemplate, ProcessFascicleTemplateValidator>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public IsValidDatePeriod(NameValueCollection attributes)
            : base("Il periodo delle date non è valido.", nameof(IsValidDatePeriod))
        {

        }

        #endregion 

        protected override void ValidateObject(ProcessFascicleTemplateValidator objectToValidate)
        {
            if (objectToValidate.StartDate.Date.Date > (objectToValidate.EndDate.HasValue ? objectToValidate.EndDate.Value : DateTime.Today))
            {
                GenerateInvalidateResult();
            }
        }
    }
}
