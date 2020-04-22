using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Dossiers
{
    public class DossierRoleTableValuedModelMapper : BaseModelMapper<DossierTableValuedModel, DossierRoleModel>, IDossierRoleTableValuedModelMapper
    {

        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public DossierRoleTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override DossierRoleModel Map(DossierTableValuedModel model, DossierRoleModel modelTransformed)
        {
            modelTransformed.Role = _mapperUnitOfWork.Repository<IDomainMapper<IRoleTableValuedModel, RoleModel>>().Map(model, null);

            return modelTransformed;
        }
    }
}
