using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Templates
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasTemplateValidSigner : BaseValidator<TemplateCollaboration, TemplateCollaborationValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasTemplateValidSigner(NameValueCollection attributes)
            : base("Tutti i firmatari devono essere validi per poter pubblicare il Template.", nameof(HasTemplateValidSigner))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(TemplateCollaborationValidator objectToValidate)
        {
            int count = 0;

            count = objectToValidate.TemplateCollaborationUsers == null ? -1 : CurrentUnitOfWork.Repository<TemplateCollaboration>().CountInvalidTemplateUsers(objectToValidate.UniqueId);

            if (count < 0 || (count > 0 && objectToValidate.IsLocked == true))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
