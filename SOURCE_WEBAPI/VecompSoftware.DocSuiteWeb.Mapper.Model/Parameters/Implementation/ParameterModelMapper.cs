using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.Parameter;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Parameters
{
    public class ParameterModelMapper : BaseModelMapper<Parameter, ParameterModel>, IParameterModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public ParameterModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #region [ Methods ]

        public override ParameterModel Map(Parameter entity, ParameterModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.LastUsedYear = entity.LastUsedYear;
            modelTransformed.LastUsedNumber = entity.LastUsedNumber;
            modelTransformed.Locked = entity.Locked;
            modelTransformed.LastUsedidCategory = entity.LastUsedIdCategory;
            modelTransformed.LastUsedidContainer = entity.LastUsedIdContainer;
            modelTransformed.LastUsedidRole = entity.LastUsedIdRole;
            modelTransformed.LastUsedIdRoleUser = entity.LastUsedIdRoleUser;
            modelTransformed.LastUsedidResolution = entity.LastUsedIdResolution;
            modelTransformed.LastUsedResolutionYear = entity.LastUsedResolutionYear;
            modelTransformed.LastUsedResolutionNumber = entity.LastUsedResolutionNumber;
            modelTransformed.LastUsedBillNumber = entity.LastUsedBillNumber;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.RegistrationDate = entity.RegistrationDate;

            modelTransformed.TenantAOO = entity.TenantAOO != null ? _mapperUnitOfWork.Repository<IDomainMapper<TenantAOO, TenantAOOModel>>().Map(entity.TenantAOO, new TenantAOOModel()) : null;

            return modelTransformed;
        }
        #endregion
    }
}
