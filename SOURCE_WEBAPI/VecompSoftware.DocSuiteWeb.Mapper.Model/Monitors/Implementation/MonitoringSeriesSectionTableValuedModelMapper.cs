using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Monitors
{
    public class MonitoringSeriesSectionTableValuedModelMapper : BaseModelMapper<MonitoringSeriesSectionTableValuedModel, MonitoringSeriesSectionModel>, IMonitoringSeriesSectionTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public MonitoringSeriesSectionTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        public override MonitoringSeriesSectionModel Map(MonitoringSeriesSectionTableValuedModel model, MonitoringSeriesSectionModel modelTransformed)
        {
            modelTransformed.Family = model.Family;
            modelTransformed.Series = model.Series;
            modelTransformed.SubSection = model.SubSection;
            modelTransformed.ActivePublished = model.ActivePublished;
            modelTransformed.Inserted = model.Inserted;
            modelTransformed.Published = model.Published;
            modelTransformed.Updated = model.Updated;
            modelTransformed.Canceled = model.Canceled;
            modelTransformed.Retired = model.Retired;
            modelTransformed.LastUpdated = model.LastUpdated;

            return modelTransformed;
        }
        public override ICollection<MonitoringSeriesSectionModel> MapCollection(ICollection<MonitoringSeriesSectionTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<MonitoringSeriesSectionModel>();
            }
            List<MonitoringSeriesSectionModel> modelsTransformed = new List<MonitoringSeriesSectionModel>();
            MonitoringSeriesSectionModel modelTransformed = null;
            foreach (IGrouping<int, MonitoringSeriesSectionTableValuedModel> monitoringSeriesSectionLookup in model)
            {
                modelTransformed = Map(monitoringSeriesSectionLookup.First(), new MonitoringSeriesSectionModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
