using VecompSoftware.DocSuiteWeb.Model.Entities.Parameter;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Parameters
{
    public class ParameterTableValuedMapper : BaseModelMapper<ParameterTableValuedModel, ParameterTableValuedModel>, IParameterTableValuedMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public ParameterTableValuedMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;

        }

        public override ParameterTableValuedModel Map(ParameterTableValuedModel model, ParameterTableValuedModel modelTransformed)
        {
            modelTransformed.UniqueId = model.UniqueId;
            modelTransformed.LastUsedYear = model.LastUsedYear;
            modelTransformed.LastUsedNumber = model.LastUsedNumber;
            modelTransformed.Locked = model.Locked;
            modelTransformed.LastUsedidCategory = model.LastUsedidCategory;
            modelTransformed.LastUsedidContainer = model.LastUsedidContainer;
            modelTransformed.LastUsedidRole = model.LastUsedidRole;
            modelTransformed.LastUsedIdRoleUser = model.LastUsedIdRoleUser;
            modelTransformed.LastUsedidResolution = model.LastUsedidResolution;
            modelTransformed.LastUsedResolutionYear = model.LastUsedResolutionYear;
            modelTransformed.LastUsedResolutionNumber = model.LastUsedResolutionNumber;
            modelTransformed.LastUsedBillNumber = model.LastUsedBillNumber;
            modelTransformed.RegistrationUser = model.RegistrationUser;
            modelTransformed.RegistrationDate = model.RegistrationDate;
            modelTransformed.TenantAOO_IdTenantAOO = model.TenantAOO_IdTenantAOO;
            model.TenantAOO_Name = model.TenantAOO_Name;

            return modelTransformed;
        }

    }
}
