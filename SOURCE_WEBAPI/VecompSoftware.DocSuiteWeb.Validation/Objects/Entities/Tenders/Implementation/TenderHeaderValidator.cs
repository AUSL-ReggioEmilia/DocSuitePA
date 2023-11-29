using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenders;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenders
{
    public class TenderHeaderValidator : ObjectValidator<TenderHeader, TenderHeaderValidator>, ITenderHeaderValidator
    {
        #region [ Constructor ]
        public TenderHeaderValidator(ILogger logger, ITenderHeaderValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvService)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvService)
        {
            TenderLots = new Collection<TenderLot>();
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public int? Year { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public DocumentSeriesItem DocumentSeriesItem { get; set; }
        public Resolution Resolution { get; set; }
        public ICollection<TenderLot> TenderLots { get; set; }
        #endregion
    }
}
