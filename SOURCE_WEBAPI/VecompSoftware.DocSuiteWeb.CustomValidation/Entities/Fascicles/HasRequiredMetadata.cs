using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasRequiredMetadata : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasRequiredMetadata(NameValueCollection attributes)
            : base("Alcuni metadati dinamici obbligatori sono vuoti.", nameof(HasRequiredMetadata))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            bool result = false;
            MetadataDesignerModel metadataModel = !string.IsNullOrEmpty(objectToValidate.MetadataDesigner) ? JsonConvert.DeserializeObject<MetadataDesignerModel>(objectToValidate.MetadataDesigner) : null;
            ICollection<MetadataValueModel> metadataValueModels = !string.IsNullOrEmpty(objectToValidate.MetadataValues) ? JsonConvert.DeserializeObject<ICollection<MetadataValueModel>>(objectToValidate.MetadataValues) : null;
            if (metadataModel != null && metadataValueModels != null &&
                (metadataModel.TextFields.Any(t => t.Required && metadataValueModels.Any(mv => mv.KeyName == t.KeyName && string.IsNullOrEmpty(mv.Value))) ||
                 metadataModel.NumberFields.Any(t => t.Required && metadataValueModels.Any(mv => mv.KeyName == t.KeyName && string.IsNullOrEmpty(mv.Value))) ||
                 metadataModel.DateFields.Any(t => t.Required && metadataValueModels.Any(mv => mv.KeyName == t.KeyName && string.IsNullOrEmpty(mv.Value))) ||
                 metadataModel.BoolFields.Any(t => t.Required && metadataValueModels.Any(mv => mv.KeyName == t.KeyName && string.IsNullOrEmpty(mv.Value))) ||
                 metadataModel.EnumFields.Any(t => t.Required && metadataValueModels.Any(mv => mv.KeyName == t.KeyName && string.IsNullOrEmpty(mv.Value))) ||
                 metadataModel.DiscussionFields.Any(t => t.Required && (t.Comments == null || t.Comments.Count == 0))))
            {
                result = true;
            }

            if (result)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}