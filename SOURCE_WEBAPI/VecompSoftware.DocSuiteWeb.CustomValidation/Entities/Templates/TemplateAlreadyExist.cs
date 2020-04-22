using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Templates
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class TemplateAlreadyExist : BaseValidator<TemplateCollaboration, TemplateCollaborationValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public TemplateAlreadyExist(NameValueCollection attributes)
            : base("E' già presente un Template con questo nome, sceglierne uno diverso.", nameof(TemplateAlreadyExist))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(TemplateCollaborationValidator objectToValidate)
        {
            int existItem = objectToValidate.UnitOfWork.Repository<TemplateCollaboration>().CountNameAlreadyExist(objectToValidate.Name, objectToValidate.UniqueId);
            if (existItem > 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
