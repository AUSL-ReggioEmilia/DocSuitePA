using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.JeepServiceHosts;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.JeepServiceHosts
{
    public class JeepServiceHostTableValuedModelMapper : BaseModelMapper<JeepServiceHostTableValuedModel, JeepServiceHostModel>, IJeepServiceHostTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public JeepServiceHostTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override JeepServiceHostModel Map(JeepServiceHostTableValuedModel model, JeepServiceHostModel modelTransformed)
        {
            modelTransformed.UniqueId = model.UniqueId;
            modelTransformed.Hostname = model.Hostname;
            modelTransformed.IsActive = model.IsActive;
            modelTransformed.IsDefault = model.IsDefault;
            modelTransformed.RegistrationUser = model.RegistrationUser;
            modelTransformed.RegistrationDate = model.RegistrationDate;

            return modelTransformed;
        }

        public override ICollection<JeepServiceHostModel> MapCollection(ICollection<JeepServiceHostTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<JeepServiceHostModel>();
            }
            List<JeepServiceHostModel> modelsTransformed = new List<JeepServiceHostModel>();
            JeepServiceHostModel modelTransformed = null;
            foreach (IGrouping<Guid, JeepServiceHostTableValuedModel> jeepServiceHostLookup in model.ToLookup(x => x.UniqueId))
            {
                modelTransformed = Map(jeepServiceHostLookup.First(), new JeepServiceHostModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
