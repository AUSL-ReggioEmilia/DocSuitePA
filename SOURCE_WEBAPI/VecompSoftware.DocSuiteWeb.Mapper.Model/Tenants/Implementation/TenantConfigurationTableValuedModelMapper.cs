using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Tenants
{
    public class TenantConfigurationTableValuedModelMapper : BaseModelMapper<TenantConfigurationTableValuedModel, TenantConfigurationModel>, ITenantConfigurationTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public TenantConfigurationTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override TenantConfigurationModel Map(TenantConfigurationTableValuedModel model, TenantConfigurationModel modelTransformed)
        {
            modelTransformed.ConfigurationType = model.ConfigurationType;
            modelTransformed.JsonValue = model.JsonValue;
            modelTransformed.StartDate = model.StartDate;
            modelTransformed.EndDate = model.EndDate;
            modelTransformed.Note = model.Note;

            return modelTransformed;
        }

        public override ICollection<TenantConfigurationModel> MapCollection(ICollection<TenantConfigurationTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<TenantConfigurationModel>();
            }
            List<TenantConfigurationModel> modelsTransformed = new List<TenantConfigurationModel>();
            TenantConfigurationModel modelTransformed = null;
            foreach (IGrouping<int, TenantConfigurationTableValuedModel> tenantConfigurationLookup in model.ToLookup(x => x.UniqueId))
            {
                modelTransformed = Map(tenantConfigurationLookup.First(), new TenantConfigurationModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
