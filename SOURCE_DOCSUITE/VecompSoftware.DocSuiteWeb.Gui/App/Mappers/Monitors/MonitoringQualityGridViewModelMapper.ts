import MonitoringQualityGridViewModel = require('App/ViewModels/Monitors/MonitoringQualityGridViewModel');
import IMapper = require('App/Mappers/IMapper');

class MonitoringQualityGridViewModelMapper implements IMapper<MonitoringQualityGridViewModel>{
    constructor() {
    }
    public Map(source: any): MonitoringQualityGridViewModel {
        let toMap: MonitoringQualityGridViewModel = <MonitoringQualityGridViewModel>{};

        if (!source) {
            return null;
        }

        toMap.DocumentSeries = source.DocumentSeries;
        toMap.Role = source.Role;
        toMap.IdDocumentSeries = source.IdDocumentSeries;
        toMap.IdRole = source.IdRole;
        toMap.Published = source.Published;
        toMap.FromResolution = source.FromResolution;
        toMap.FromProtocol = source.FromProtocol;
        toMap.WithoutLink = source.WithoutLink;
        toMap.WithoutDocument = source.WithoutDocument;

        return toMap;
    }
}

export = MonitoringQualityGridViewModelMapper;