using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Templates
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasTemplateUniqueDocument : BaseValidator<TemplateDocumentRepository, TemplateDocumentRepositoryValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasTemplateUniqueDocument(NameValueCollection attributes)
            : base("E' già presente un Template con questo nome, sceglierne uno diverso.", nameof(HasTemplateUniqueDocument))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(TemplateDocumentRepositoryValidator objectToValidate)
        {
            int existItem = objectToValidate.UnitOfWork.Repository<TemplateDocumentRepository>().CountNameAlreadyExist(objectToValidate.Name, objectToValidate.UniqueId);
            if (existItem > 0)
            {
                GenerateInvalidateResult();
            }

        }
        #endregion
    }
}