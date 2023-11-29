using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenders;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenders
{
    public class TenderLotPaymentValidator : ObjectValidator<TenderLotPayment, TenderLotPaymentValidator>, ITenderLotPaymentValidator
    {
        #region [ Constructor ]
        public TenderLotPaymentValidator(ILogger logger, ITenderLotPaymentValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvService)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvService)
        {
            
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string PaymentKey { get; set; }
        public decimal? Amount { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public TenderLot TenderLot { get; set; }
        #endregion
    }
}
