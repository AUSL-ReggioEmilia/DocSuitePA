using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Processes
{
    public class ProcessFascicleTemplateValidatorMapper : BaseMapper<ProcessFascicleTemplate, ProcessFascicleTemplateValidator>, IProcessFascicleTemplateValidatorMapper
    {
        public override ProcessFascicleTemplateValidator Map(ProcessFascicleTemplate entity, ProcessFascicleTemplateValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.JsonModel = entity.JsonModel;
            entityTransformed.Name = entity.Name;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Process = entity.Process;
            entityTransformed.DossierFolder = entity.DossierFolder;

            #endregion

            return entityTransformed;
        }
    }
}
