using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Templates
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class TemplateFixedFolderParentInvalid : BaseValidator<TemplateCollaboration, TemplateCollaborationValidator>
    {
        public TemplateFixedFolderParentInvalid(NameValueCollection attributes)
          : base("A template fixed folder can only be inserted the root level", nameof(TemplateFixedFolderParentInvalid))
        {
        }

        protected override void ValidateObject(TemplateCollaborationValidator objectToValidate)
        {
            if (objectToValidate.RepresentationType == TemplateCollaborationRepresentationType.FixedTemplates)
            {
                // fixed templates can only be inserted at root level
                // even though the root level has an id and we can in theory have a parent insert id, 
                // there is only one root level and it's better to impose the null condition
                if (objectToValidate.ParentInsertId.HasValue)
                {
                    GenerateInvalidateResult();
                    return;
                }
            }
        }
    }
}
