using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasContainerPrefixGroupName : BaseValidator<Container, ContainerValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasContainerPrefixGroupName(NameValueCollection attributes)
            : base("Per generare in modo automatico i gruppi di un contenitore è necessario specificarne un prefisso.", nameof(HasContainerPrefixGroupName))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ContainerValidator objectToValidate)
        {
            if (objectToValidate.AutomaticSecurityGroups.HasValue && objectToValidate.AutomaticSecurityGroups.Value && string.IsNullOrEmpty(objectToValidate.PrefixSecurityGroupName))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
