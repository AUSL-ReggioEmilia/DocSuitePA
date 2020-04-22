using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Finder.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.MassimariScarto;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.MassimariScarto
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasValidChildrenEndDate : BaseValidator<MassimarioScarto, MassimarioScartoValidator>
    {

        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidChildrenEndDate(NameValueCollection attributes)
            : base("La data di disattivazione non può essere antecedente alla data di disattivazione del nodo figlio.", nameof(HasValidChildrenEndDate))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(MassimarioScartoValidator objectToValidate)
        {
            if (!objectToValidate.EndDate.HasValue)
            {
                return;
            }

            ICollection<MassimarioScarto> children = CurrentUnitOfWork.Repository<MassimarioScarto>().GetAllChildrenByParent(objectToValidate.UniqueId, true).ToList();

            if (children.Any(x => x.EndDate.HasValue && objectToValidate.EndDate.Value < x.EndDate.Value))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
