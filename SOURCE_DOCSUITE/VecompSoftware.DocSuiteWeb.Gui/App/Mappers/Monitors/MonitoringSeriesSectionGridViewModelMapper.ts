import MonitoringSeriesSectionGridViewModel = require('App/ViewModels/Monitors/MonitoringSeriesSectionGridViewModel');
import IMapper = require('App/Mappers/IMapper');

class MonitoringSeriesSectionGridViewModelMapper implements IMapper<MonitoringSeriesSectionGridViewModel>{
    constructor() {
    }
    public Map(source: any): MonitoringSeriesSectionGridViewModel {
        let toMap: MonitoringSeriesSectionGridViewModel = <MonitoringSeriesSectionGridViewModel>{};

        if (!source) {
            return null;
        }

        toMap.Family = source.Family;
        toMap.Series = source.Series;
        toMap.SubSection = source.SubSection;
        toMap.ActivePublished = source.ActivePublished;
        toMap.Inserted = source.Inserted;
        toMap.Published = source.Published;
        toMap.Updated = source.Updated;
        toMap.Canceled = source.Canceled;
        toMap.Retired = source.Retired;
        toMap.LastUpdated = moment(source.LastUpdated).format("DD/MM/YYYY");

        return toMap;
    }
}

export = MonitoringSeriesSectionGridViewModelMapper;