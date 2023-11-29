using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tasks;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tasks
{
    public class TaskHeaderValidator : ObjectValidator<TaskHeader, TaskHeaderValidator>, ITaskHeaderValidator
    {
        #region [ Constructor ]
        public TaskHeaderValidator(ILogger logger, ITaskHeaderValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
            TaskHeaderProtocols = new Collection<TaskHeaderProtocol>();
        }
        #endregion

        #region [ Properties ]
        public int EntityId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskType TaskType { get; set; }
        public TaskStatus Status { get; set; }
        public TaskHeaderSendingProcessStatus? SendingProcessStatus { get; set; }
        public TaskHeaderSendedStatus? SendedStatus { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public ICollection<TaskHeaderProtocol> TaskHeaderProtocols { get; set; }
        #endregion
    }
}
