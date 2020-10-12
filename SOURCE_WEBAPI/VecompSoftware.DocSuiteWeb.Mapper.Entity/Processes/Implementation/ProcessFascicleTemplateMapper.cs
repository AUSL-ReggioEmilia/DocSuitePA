using VecompSoftware.DocSuiteWeb.Entity.Processes;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Processes
{
    public class ProcessFascicleTemplateMapper : BaseEntityMapper<ProcessFascicleTemplate, ProcessFascicleTemplate>, IProcessFascicleTemplateMapper
    {
        public ProcessFascicleTemplateMapper()
        {
        }

        public override ProcessFascicleTemplate Map(ProcessFascicleTemplate entity, ProcessFascicleTemplate entityTransformed)
        {
            #region [ Base ]

            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Name = entity.Name;
            entityTransformed.JsonModel = entity.JsonModel;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.ObjectState = entity.ObjectState;

            #endregion

            return entityTransformed;
        }
    }
}
