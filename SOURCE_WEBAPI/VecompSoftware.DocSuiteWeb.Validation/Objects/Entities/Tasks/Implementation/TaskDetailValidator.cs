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
    public class TaskDetailValidator : ObjectValidator<TaskDetail, TaskDetailValidator>, ITaskDetailValidator
    {
        #region [ Constructor ]
        public TaskDetailValidator(ILogger logger, ITaskDetailValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
            
        }
        #endregion

        #region [ Properties ]
        public int EntityId { get; set; }
        public TaskDetailType DetailType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ErrorDescription { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public TaskHeader TaskHeader { get; set; }
        #endregion
    }
}
