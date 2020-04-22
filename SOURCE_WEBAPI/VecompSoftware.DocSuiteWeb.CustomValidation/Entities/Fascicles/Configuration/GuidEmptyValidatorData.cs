using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using VecompSoftware.DocSuiteWeb.CustomValidation.Properties;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.Configuration
{
    [ResourceDescription(typeof(Resources), "GuidEmptyValidatorDescription")]
    [ResourceDisplayName(typeof(Resources), "GuidEmptyValidatorName")]
    public class GuidEmptyValidatorData : ValueValidatorData
    {
        public GuidEmptyValidatorData()
        {
            Type = typeof(GuidEmptyValidator);
        }

        public GuidEmptyValidatorData(string name)
            : base(name, typeof(GuidEmptyValidator))
        {
        }


        protected override Validator DoCreateValidator(Type targetType)
        {
            return new GuidEmptyValidator(Tag);
        }
    }
}