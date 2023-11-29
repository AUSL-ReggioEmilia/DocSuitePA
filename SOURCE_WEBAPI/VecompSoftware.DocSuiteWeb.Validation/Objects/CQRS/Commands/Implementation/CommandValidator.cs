using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.CQRS.Commands
{
    public class CommandValidator : ObjectValidator<ICommand, CommandValidator>, ICommandValidator
    {
        #region [ Constructor ]
        public CommandValidator(ILogger logger, ICommandValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService  parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region Properties

        public string CommandName { get; set; }

        public string TenantName { get; set; }

        public Guid? TenantId { get; set; }

        public Guid? CommandId { get; set; }

        public DateTimeOffset? CreationTime { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
