using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Commons
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class ContainerNameAlreadyExist : BaseValidator<Container, ContainerValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ] 
        public ContainerNameAlreadyExist(NameValueCollection attributes)
            : base("Un Conentitore con il nome scelto è già esistente.", nameof(ContainerNameAlreadyExist))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ContainerValidator objectToValidate)
        {
            int count = objectToValidate == null ? 0 : CurrentUnitOfWork.Repository<Container>().CountContainerByName(objectToValidate.Name);

            if (count > 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
