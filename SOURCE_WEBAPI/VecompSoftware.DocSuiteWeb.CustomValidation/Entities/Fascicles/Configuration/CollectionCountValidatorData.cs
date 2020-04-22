using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using VecompSoftware.DocSuiteWeb.CustomValidation.Properties;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles.Configuration
{
    [ResourceDescription(typeof(Resources), "CollectionCountValidatorDescription")]
    [ResourceDisplayName(typeof(Resources), "CollectionCountValidatorName")]
    public class CollectionCountValidatorData : ValueValidatorData
    {
        public CollectionCountValidatorData()
        {
            Type = typeof(CollectionCountValidator);
        }

        public CollectionCountValidatorData(string name)
            : base(name, typeof(CollectionCountValidator))
        {
        }


        protected override Validator DoCreateValidator(Type targetType)
        {
            return new CollectionCountValidator(Tag);
        }
    }
}