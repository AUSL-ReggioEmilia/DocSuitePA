using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.UDS
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class DocumentUnitAlreadyExists : BaseValidator<UDSDocumentUnit, UDSDocumentUnitValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public DocumentUnitAlreadyExists(NameValueCollection attributes)
            : base("L'unità documentale esiste già nell'archivio specificato.", nameof(DocumentUnitAlreadyExists))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(UDSDocumentUnitValidator objectToValidate)
        {
            bool res = CurrentUnitOfWork.Repository<UDSDocumentUnit>().GetByUDSIdAndDocumentUnit(objectToValidate.IdUDS, objectToValidate.DocumentUnit.UniqueId).Any();
            if (res)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
