using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.CQRS.Events
{
    public class EventValidator : ObjectValidator<IEvent, EventValidator>, IEventValidator
    {
        #region [ Constructor ]
        public EventValidator(ILogger logger, IEventValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region Properties

        public string EventName { get; set; }

        public string TenantName { get; set; }

        public Guid? TenantId { get; set; }

        public Guid? EventId { get; set; }

        public DateTimeOffset? CreationTime { get; set; }

        public DateTimeOffset ExecutedTime { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
