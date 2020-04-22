using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Dossiers
{
    public class DossierRoleModelMapper : BaseModelMapper<DossierRole, DossierRoleModel>, IDossierRoleModelMapper
    {
        #region [ Fields ]

        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        #endregion

        #region [ Constructor ]
        public DossierRoleModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]

        public override DossierRoleModel Map(DossierRole entity, DossierRoleModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.Role = entity.Role == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<Role, RoleModel>>().Map(entity.Role, new RoleModel());

            return modelTransformed;
        }

        #endregion
    }
}
