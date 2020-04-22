using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Fascicles
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasFascicleDocumentGuidEmpty : BaseValidator<Fascicle, FascicleValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasFascicleDocumentGuidEmpty(NameValueCollection attributes)
            : base("Gli inserti non possiedono un archivio valido", nameof(HasFascicleDocumentGuidEmpty))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(FascicleValidator objectToValidate)
        {
            if (objectToValidate.FascicleDocuments.Any(x => x.IdArchiveChain == Guid.Empty))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
