using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Templates
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class TemplateFolderParentInvalid : BaseValidator<TemplateCollaboration, TemplateCollaborationValidator>
    {
        public TemplateFolderParentInvalid(NameValueCollection attributes)
          : base("A template folder must have a parent of type Folder of FixedTemplate", nameof(TemplateFolderParentInvalid))
        {
        }

        protected override void ValidateObject(TemplateCollaborationValidator objectToValidate)
        {
            if (objectToValidate.RepresentationType == TemplateCollaborationRepresentationType.Folder)
            {
                if (!objectToValidate.ParentInsertId.HasValue)
                {
                    GenerateInvalidateResult();
                    return;
                }

                TemplateCollaboration parentFolder
                    = objectToValidate.UnitOfWork.Repository<TemplateCollaboration>().Find(objectToValidate.ParentInsertId);

                if (parentFolder.RepresentationType != TemplateCollaborationRepresentationType.FixedTemplates
                    && parentFolder.RepresentationType != TemplateCollaborationRepresentationType.Folder)
                {
                    GenerateInvalidateResult();
                }
            }
        }
    }
}
