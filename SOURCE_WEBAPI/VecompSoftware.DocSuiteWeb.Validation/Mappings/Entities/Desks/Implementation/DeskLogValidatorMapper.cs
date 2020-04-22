using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks
{
    public class DeskLogValidatorMapper : BaseMapper<DeskLog, DeskLogValidator>, IDeskLogValidatorMapper
    {
        public DeskLogValidatorMapper() { }

        public override DeskLogValidator Map(DeskLog entity, DeskLogValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Desk = entity.Entity;
            #endregion

            return entityTransformed;
        }
    }
}
