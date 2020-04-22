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
            modelTransformed.Password = model.Password;
            modelTransformed.LastUsedidCategory = model.LastUsedidCategory;
            modelTransformed.LastUsedidRecipient = model.LastUsedidRecipient;
            modelTransformed.LastUsedidContainer = model.LastUsedidContainer;
            modelTransformed.Version = model.Version;
            modelTransformed.LastUsedidDistributionList = model.LastUsedidDistributionList;
            modelTransformed.DomainName = model.DomainName;
            modelTransformed.AlternativePassword = model.AlternativePassword;
            modelTransformed.ServiceField = model.ServiceField;
            modelTransformed.LastUsedidRole = model.LastUsedidRole;
            modelTransformed.LastUsedIdRoleUser = model.LastUsedIdRoleUser;
            modelTransformed.LastUsedidResolution = model.LastUsedidResolution;
            modelTransformed.LastUsedResolutionYear = model.LastUsedResolutionYear;
            modelTransformed.LastUsedResolutionNumber = model.LastUsedResolutionNumber;
            modelTransformed.LastUsedBillNumber = model.LastUsedBillNumber;
            modelTransformed.LastUsedYearReg = model.LastUsedYearReg;
            modelTransformed.LastUsedNumberReg = model.LastUsedNumberReg;
            modelTransformed.RegistrationUser = model.RegistrationUser;
            modelTransformed.RegistrationDate = model.RegistrationDate;

            return modelTransformed;
        }

    }
}
