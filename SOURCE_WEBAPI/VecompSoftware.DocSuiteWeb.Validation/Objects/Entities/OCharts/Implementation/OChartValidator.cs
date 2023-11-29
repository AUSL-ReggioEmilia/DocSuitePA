using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.OCharts;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.OCharts
{
    public class OChartValidator : ObjectValidator<OChart, OChartValidator>, IOChartValidator
    {
        #region [ Constructor ]
        public OChartValidator(ILogger logger, IOChartValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? Imported { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public ICollection<OChartItem> OChartItems { get; set; }

        #endregion
    }
}
