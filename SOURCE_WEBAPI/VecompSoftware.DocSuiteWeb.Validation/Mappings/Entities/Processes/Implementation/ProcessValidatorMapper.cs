using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Processes
{
    public class ProcessValidatorMapper : BaseMapper<Process, ProcessValidator>, IProcessValidatorMapper
    {
        public ProcessValidatorMapper()
        {
        }

        public override ProcessValidator Map(Process entity, ProcessValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.FascicleType = entity.FascicleType;
            entityTransformed.Note = entity.Note;
            entityTransformed.Name = entity.Name;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.FascicleTemplates = entity.FascicleTemplates;
            entityTransformed.FascicleWorkflowRepositories = entity.FascicleWorkflowRepositories;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Dossier = entity.Dossier;
            entityTransformed.Category = entity.Category;

            #endregion

            return entityTransformed;
        }
    }
}
