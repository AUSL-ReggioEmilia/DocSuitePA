using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.OCharts;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.OCharts
{
    public class OChartItemContainerValidator : ObjectValidator<OChartItemContainer, OChartItemContainerValidator>, IOChartItemContainerValidator
    {
        #region [ Constructor ]
        public OChartItemContainerValidator(ILogger logger, IOChartItemContainerValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public bool? Master { get; set; }
        public bool? Rejection { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public OChartItem OChartItem { get; set; }
        public Container Container { get; set; }

        #endregion
    }
}
