using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Monitors;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Monitors
{
    public class TransparentAdministrationMonitorLogValidatorMapper : BaseMapper<TransparentAdministrationMonitorLog, TransparentAdministrationMonitorLogValidator>, ITransparentAdministrationMonitorLogValidatorMapper
    {
        public TransparentAdministrationMonitorLogValidatorMapper() { }

        public override TransparentAdministrationMonitorLogValidator Map(TransparentAdministrationMonitorLog entity, TransparentAdministrationMonitorLogValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.DocumentUnitName = entity.DocumentUnitName;
            entityTransformed.Date = entity.Date;
            entityTransformed.Note = entity.Note;
            entityTransformed.Rating = entity.Rating;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocumentUnit = entity.DocumentUnit;
            entityTransformed.Role = entity.Role;
            #endregion

            return entityTransformed;
        }
    }
}
