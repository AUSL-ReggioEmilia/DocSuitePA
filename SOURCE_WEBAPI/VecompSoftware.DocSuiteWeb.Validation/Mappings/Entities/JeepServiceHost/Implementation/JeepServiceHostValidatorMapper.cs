using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.JeepServiceHosts;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.JeepServiceHost
{
    public class JeepServiceHostValidatorMapper : BaseMapper<Entity.JeepServiceHosts.JeepServiceHost, JeepServiceHostValidator>, IJeepServiceHostValidatorMapper
    {
        public JeepServiceHostValidatorMapper() { }

        public override JeepServiceHostValidator Map(Entity.JeepServiceHosts.JeepServiceHost entity, JeepServiceHostValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Hostname = entity.Hostname;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.IsDefault = entity.IsDefault;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }
    }
}
