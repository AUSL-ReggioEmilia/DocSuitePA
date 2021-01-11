using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tasks;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tasks
{
    public class TaskHeaderProtocolValidator : ObjectValidator<TaskHeaderProtocol, TaskHeaderProtocolValidator>, ITaskHeaderProtocolValidator
    {
        #region [ Constructor ]
        public TaskHeaderProtocolValidator(ILogger logger, ITaskHeaderProtocolValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        {
        }
        #endregion

        #region [ Properties ]
        public int EntityId { get; set; }
        public short? Year { get; set; }
        public int? Number { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public TaskHeader TaskHeader { get; set; }
        public Protocol Protocol { get; set; }
        #endregion
    }
}
