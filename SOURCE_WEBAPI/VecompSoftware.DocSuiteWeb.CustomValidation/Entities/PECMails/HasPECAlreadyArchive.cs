using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.PECMails
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class HasPECAlreadyArchive : BaseValidator<PECMail, PECMailValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public HasPECAlreadyArchive(NameValueCollection attributes)
            : base("La PEC indicata risulta già archiviata in una unità documentale.", nameof(HasPECAlreadyArchive))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(PECMailValidator objectToValidate)
        {
            PECMail pec = CurrentUnitOfWork.Repository<PECMail>().Queryable(true).SingleOrDefault(x => x.EntityId == objectToValidate.EntityId);
            if (pec.Year.HasValue && pec.Number.HasValue)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
