using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenders;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenders
{
    public class TenderLotValidator : ObjectValidator<TenderLot, TenderLotValidator>, ITenderLotValidator
    {
        #region [ Constructor ]
        public TenderLotValidator(ILogger logger, ITenderLotValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvService)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvService)
        {
            TenderLotPayments = new Collection<TenderLotPayment>();
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string CIG { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public TenderHeader TenderHeader { get; set; }
        public ICollection<TenderLotPayment> TenderLotPayments { get; set; }
        #endregion
    }
}
