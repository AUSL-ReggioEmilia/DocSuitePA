using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Templates
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class TemplateParentInvalid : BaseValidator<TemplateCollaboration, TemplateCollaborationValidator>
    {
        public TemplateParentInvalid(NameValueCollection attributes)
            : base("A template must have a parent of type Folder of FixedTemplate", nameof(TemplateParentInvalid))
        {
        }

        protected override void ValidateObject(TemplateCollaborationValidator objectToValidate)
        {
            if (objectToValidate.RepresentationType == TemplateCollaborationRepresentationType.Template)
            {
                if (!objectToValidate.ParentInsertId.HasValue)
                {
                    GenerateInvalidateResult();
                    return;
                }

                TemplateCollaboration parentFolder
                    = objectToValidate.UnitOfWork.Repository<TemplateCollaboration>().Find(objectToValidate.ParentInsertId);

                // a fixed template is a template type that is mounted just under the root level and 
                // it can have folders and sub-templates
                if (parentFolder.RepresentationType != TemplateCollaborationRepresentationType.FixedTemplates
                    && parentFolder.RepresentationType != TemplateCollaborationRepresentationType.Folder)
                {
                    GenerateInvalidateResult();
                }
            }
        }
    }
}
