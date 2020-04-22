using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Model.Processes;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Processes
{
    public class ProcessFascicleTemplateModelMapper : BaseModelMapper<ProcessFascicleTemplate, ProcessFascicleTemplateModel>, IProcessFascicleTemplateModelMapper
    {
        public override ProcessFascicleTemplateModel Map(ProcessFascicleTemplate entity, ProcessFascicleTemplateModel entityTransformed)
        {
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.JsonModel = entity.JsonModel;
            entityTransformed.Name = entity.Name;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;

            return entityTransformed;
        }
    }
}