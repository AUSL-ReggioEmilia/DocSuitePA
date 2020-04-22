using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.MetadataRepositories
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class MetadataRepositoryAlreadyExists : BaseValidator<MetadataRepository, MetadataRepositoryValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public MetadataRepositoryAlreadyExists(NameValueCollection attributes)
            : base("E' già presente un deposito di metadati con lo stesso nome.", nameof(MetadataRepositoryAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(MetadataRepositoryValidator objectToValidate)
        {
            int res = CurrentUnitOfWork.Repository<MetadataRepository>().FindExistingRepository(objectToValidate.Name);

            if (res > 0)
            {
                GenerateInvalidateResult();
            }

        }
        #endregion
    }
}
