using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using Newtonsoft.Json;
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
            MetadataModel metadataModel = objectToValidate.MetadataValues != null ? JsonConvert.DeserializeObject<MetadataModel>(objectToValidate.MetadataValues) : null;
            if (metadataModel != null &&
                (metadataModel.TextFields.Any(t => t.Required && string.IsNullOrEmpty(t.Value)) ||
                 metadataModel.NumberFields.Any(t => t.Required && string.IsNullOrEmpty(t.Value)) ||
                 metadataModel.DateFields.Any(t => t.Required && string.IsNullOrEmpty(t.Value)) ||
                 metadataModel.BoolFields.Any(t => t.Required && string.IsNullOrEmpty(t.Value)) ||
                 metadataModel.EnumFields.Any(t => t.Required && string.IsNullOrEmpty(t.Value)) ||
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