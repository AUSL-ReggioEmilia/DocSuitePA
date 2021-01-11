using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class PrivacyLevelValidatorMapper : BaseMapper<PrivacyLevel, PrivacyLevelValidator>, IPrivacyLevelValidatorMapper
    {
        public PrivacyLevelValidatorMapper() { }

        public override PrivacyLevelValidator Map(PrivacyLevel entity, PrivacyLevelValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Level = entity.Level;
            entityTransformed.Description = entity.Description;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.Colour = entity.Colour;
            #endregion

            #region [ Navigation Properties ]       
            #endregion

            return entityTransformed;
        }

    }
}
