using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System;
using System.Collections.Specialized;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.MassimariScarto;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.MassimariScarto
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasValidStartDate : BaseValidator<MassimarioScarto, MassimarioScartoValidator>
    {

        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidStartDate(NameValueCollection attributes)
            : base("La data di attivazione non può essere antecedente alla data di attivazione del nodo padre o posteriore alla data di disattivazione del nodo padre.", nameof(HasValidStartDate))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(MassimarioScartoValidator objectToValidate)
        {
            MassimarioScarto parent = null;

            if (objectToValidate.FakeInsertId.HasValue)
            {
                parent = objectToValidate.StartDate == null || objectToValidate.StartDate == DateTimeOffset.MinValue ? null : CurrentUnitOfWork.Repository<MassimarioScarto>().Find(objectToValidate.FakeInsertId.Value);
            }

            if (parent == null || (parent != null && parent.MassimarioScartoLevel != 0 && ((parent.StartDate > objectToValidate.StartDate) || (parent.EndDate.HasValue && parent.EndDate < objectToValidate.StartDate))))
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
