using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Finder.Processes;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Processes.ProcessFascicleTemplates
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsProcessFascicleTemplateNameUnique : BaseValidator<ProcessFascicleTemplate, ProcessFascicleTemplateValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public IsProcessFascicleTemplateNameUnique(NameValueCollection attributes)
            : base("Il template con il nome scelto è già stato creato.", nameof(IsProcessFascicleTemplateNameUnique))
        {
        }

        #endregion

        #region [ Methods ]

        protected override void ValidateObject(ProcessFascicleTemplateValidator objectToValidate)
        {
            bool result = objectToValidate.DossierFolder == null ? false : CurrentUnitOfWork.Repository<ProcessFascicleTemplate>().NameAlreadyExists(objectToValidate.Name, objectToValidate.DossierFolder.UniqueId);
            if (result)
            {
                GenerateInvalidateResult();
            }
        }

        #endregion
    }
}
