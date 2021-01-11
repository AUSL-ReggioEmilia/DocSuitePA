using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class PrivacyLevelMapper : BaseEntityMapper<PrivacyLevel, PrivacyLevel>, IPrivacyLevelMapper
    {
        public PrivacyLevelMapper()
        {
        }

        public override PrivacyLevel Map(PrivacyLevel entity, PrivacyLevel entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Description = entity.Description;
            entityTransformed.Level = entity.Level;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.Colour = entity.Colour;
            #endregion

            return entityTransformed;
        }

    }
}
