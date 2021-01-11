using VecompSoftware.DocSuiteWeb.Entity.Monitors;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Monitors
{
    public class TransparentAdministrationMonitorLogMapper : BaseEntityMapper<TransparentAdministrationMonitorLog, TransparentAdministrationMonitorLog>, ITransparentAdministrationMonitorLogMapper
    {
        public override TransparentAdministrationMonitorLog Map(TransparentAdministrationMonitorLog entity, TransparentAdministrationMonitorLog entityTransformed)
        {
            #region [ Base ]
            entityTransformed.DocumentUnitName = entity.DocumentUnitName;
            entityTransformed.Date = entity.Date;
            entityTransformed.Note = entity.Note;
            entityTransformed.Rating = entity.Rating;
            #endregion

            return entityTransformed;
        }
    }
}
