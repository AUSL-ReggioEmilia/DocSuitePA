using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Parameter;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Parameters
{
    public class ParameterTableValuedModelMapper : BaseModelMapper<ParameterTableValuedModel, ParameterModel>, IParameterTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public ParameterTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        public override ParameterModel Map(ParameterTableValuedModel model, ParameterModel modelTransformed)
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
        public override ICollection<ParameterModel> MapCollection(ICollection<ParameterTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<ParameterModel>();
            }
            List<ParameterModel> modelsTransformed = new List<ParameterModel>();
            ParameterModel modelTransformed = null;
            foreach (IGrouping<int, ParameterTableValuedModel> transparentAdministrationMonitorLookup in model.ToLookup(x => x.UniqueId))
            {
                modelTransformed = Map(transparentAdministrationMonitorLookup.First(), new ParameterModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }

    }
}
