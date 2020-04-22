using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.Protocols
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsCategoryActive : BaseValidator<Protocol, ProtocolValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsCategoryActive(NameValueCollection attributes)
            : base("Il protocollo non hanno categorie attive .", nameof(IsCategoryActive))
        {

        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(ProtocolValidator objectToValidate)
        {
            Category category = null;
            if (objectToValidate.Category != null)
            {
                category = CurrentUnitOfWork.Repository<Category>().GetById(objectToValidate.Category.EntityShortId).FirstOrDefault();
            }
            if (category == null ||
                (category != null && category.IsActive.HasValue && category.IsActive.Value == ActiveType.Disabled))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
