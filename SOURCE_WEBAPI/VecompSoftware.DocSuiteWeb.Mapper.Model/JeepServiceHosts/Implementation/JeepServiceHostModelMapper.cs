using VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Model.Entities.JeepServiceHosts;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.JeepServiceHosts
{
    public class JeepServiceHostModelMapper : BaseModelMapper<JeepServiceHost, JeepServiceHostModel>, IJeepServiceHostModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public JeepServiceHostModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #region [ Methods ]
        public override JeepServiceHostModel Map(JeepServiceHost entity, JeepServiceHostModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.Hostname = entity.Hostname;
            modelTransformed.IsActive = entity.IsActive;
            modelTransformed.IsDefault = entity.IsDefault;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.RegistrationUser = entity.RegistrationUser;

            return modelTransformed;
        }
        #endregion
    }
}
