using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Processes
{
    public class ProcessFascicleWorkflowRepositoryValidatorMapper : BaseMapper<ProcessFascicleWorkflowRepository, ProcessFascicleWorkflowRepositoryValidator>, IProcessFascicleWorkflowRepositoryValidatorMapper
    {
        public override ProcessFascicleWorkflowRepositoryValidator Map(ProcessFascicleWorkflowRepository entity, ProcessFascicleWorkflowRepositoryValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Process = entity.Process;
            entityTransformed.DossierFolder = entity.DossierFolder;
            entityTransformed.WorkflowRepository = entity.WorkflowRepository;

            #endregion

            return entityTransformed;
        }
    }
}
