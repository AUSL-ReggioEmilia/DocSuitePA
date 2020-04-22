using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Conservations;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Conservations
{
    public class ConservationValidatorMapper : BaseMapper<Conservation, ConservationValidator>, IConservationValidatorMapper
    {
        public ConservationValidatorMapper() { }

        public override ConservationValidator Map(Conservation entity, ConservationValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.EntityType = entity.EntityType;
            entityTransformed.Status = entity.Status;
            entityTransformed.Message = entity.Message;
            entityTransformed.Type = entity.Type;
            entityTransformed.SendDate = entity.SendDate;
            entityTransformed.Uri = entity.Uri;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            return entityTransformed;
        }

    }
}
