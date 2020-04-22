import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');
import MonitoringQualityGridViewModel = require('App/ViewModels/Monitors/MonitoringQualityGridViewModel');
import MonitoringQualityGridViewModelMapper = require('App/Mappers/Monitors/MonitoringQualityGridViewModelMapper');
import MonitoringQualitySearchFilterDTO = require('App/DTOs/MonitoringQualitySearchFilterDTO');

class MonitoringQualityService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getMonitoringQualitySummary(searchFilter: MonitoringQualitySearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.
            concat("/DocumentSeriesService.GetMonitoringQualitySummary(dateFrom='", searchFilter.dateFrom,
                "',dateTo='", searchFilter.dateTo, "')");
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let viewModelMapper = new MonitoringQualityGridViewModelMapper();
                let monitoringSeriesSection: MonitoringQualityGridViewModel[] = [];
                $.each(response.value, function (i, value) {
                    monitoringSeriesSection.push(viewModelMapper.Map(value));
                });
                callback(monitoringSeriesSection);
            };
        }, error);
    }
}

export = MonitoringQualityService;