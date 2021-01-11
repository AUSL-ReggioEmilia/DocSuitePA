using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Templates
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasTemplateValidRole : BaseValidator<TemplateCollaboration, TemplateCollaborationValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasTemplateValidRole(NameValueCollection attributes)
            : base("Il settore selezionato non è attivo.", nameof(HasTemplateValidRole))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(TemplateCollaborationValidator objectToValidate)
        {
            int count = 0;

            count = objectToValidate.TemplateCollaborationUsers == null ? 1 : CurrentUnitOfWork.Repository<TemplateCollaboration>().CountInvalidTemplateRoles(objectToValidate.UniqueId);

            if (count > 0)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
