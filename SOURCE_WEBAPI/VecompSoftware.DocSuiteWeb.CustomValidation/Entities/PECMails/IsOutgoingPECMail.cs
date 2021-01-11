using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using System.Collections.Specialized;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Finder.PECMails;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.CustomValidation.Entities.PECMails
{
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class IsOutgoingPECMail : BaseValidator<PECMail, PECMailValidator>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public IsOutgoingPECMail(NameValueCollection attributes)
            : base("L'inserimento è disponibile solo per il tipo in uscita .", nameof(IsOutgoingPECMail))
        {
        }
        #endregion

        #region [ Methods ]
        protected override void ValidateObject(PECMailValidator objectToValidate)
        {
            PECMail pecMail = CurrentUnitOfWork.Repository<PECMail>().GetByUniqueId(objectToValidate.UniqueId).First();

            if (pecMail != null && pecMail.Direction != PECMailDirection.Outgoing)
            {
                GenerateInvalidateResult();
            }
        }
        #endregion
    }
}
