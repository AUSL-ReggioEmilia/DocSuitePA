using VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.JeepServiceHosts
{
    public class JeepServiceHostMapper : BaseEntityMapper<JeepServiceHost, JeepServiceHost>, IJeepServiceHostMapper
    {
        public override JeepServiceHost Map(JeepServiceHost entity, JeepServiceHost entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Hostname = entity.Hostname;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.IsDefault = entity.IsDefault;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            #endregion

            return entityTransformed;
        }
    }
}
