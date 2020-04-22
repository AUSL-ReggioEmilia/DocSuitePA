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
    public class HasValidCode : BaseValidator<MassimarioScarto, MassimarioScartoValidator>
    {

        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasValidCode(NameValueCollection attributes)
            : base("E' già presente un altro massimario di scarto con lo stesso codice.", nameof(HasValidCode))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(MassimarioScartoValidator objectToValidate)
        {
            MassimarioScarto parent = null;
            MassimarioScarto children = null;

            if (objectToValidate.FakeInsertId.HasValue)
            {
                parent = CurrentUnitOfWork.Repository<MassimarioScarto>().Find(objectToValidate.FakeInsertId.Value);
                ICollection<short> codes = new List<short>();

                if (parent != null && parent.Code.HasValue)
                {
                    codes.Add(parent.Code.Value);
                }

                if (objectToValidate.Code.HasValue)
                {
                    codes.Add(objectToValidate.Code.Value);
                }

                children = CurrentUnitOfWork.Repository<MassimarioScarto>().GetByCode(string.Join("|", codes)).FirstOrDefault();

            }

            if (parent == null || !objectToValidate.Code.HasValue || children != null)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
