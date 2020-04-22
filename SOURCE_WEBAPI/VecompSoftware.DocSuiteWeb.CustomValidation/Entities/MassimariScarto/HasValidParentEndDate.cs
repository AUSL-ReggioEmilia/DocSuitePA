using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Finder.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.MassimariScarto;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.MassimariScarto
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasValidParentEndDate : BaseValidator<MassimarioScarto, MassimarioScartoValidator>
    {

        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidParentEndDate(NameValueCollection attributes)
            : base("La data di disattivazione non può essere superiore alla data di disattivazione del nodo padre.", nameof(HasValidParentEndDate))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(MassimarioScartoValidator objectToValidate)
        {
            MassimarioScarto parent = CurrentUnitOfWork.Repository<MassimarioScarto>().GetParentByPath(objectToValidate.MassimarioScartoParentPath).FirstOrDefault();

            if (parent == null || (parent != null && parent.MassimarioScartoLevel != 0 && (objectToValidate.EndDate.HasValue && parent.EndDate.HasValue && parent.EndDate.Value < objectToValidate.EndDate.Value)))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
