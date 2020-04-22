using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks
{
    public class DeskMessageValidatorMapper : BaseMapper<DeskMessage, DeskMessageValidator>, IDeskMessageValidatorMapper
    {
        public DeskMessageValidatorMapper() { }

        public override DeskMessageValidator Map(DeskMessage entity, DeskMessageValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Desk = entity.Desk;
            entityTransformed.Message = entity.Message;
            #endregion

            return entityTransformed;
        }

    }
}
