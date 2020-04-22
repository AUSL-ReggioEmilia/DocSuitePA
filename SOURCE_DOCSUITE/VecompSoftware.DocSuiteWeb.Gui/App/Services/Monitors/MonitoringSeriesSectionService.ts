import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');
import MonitoringSeriesSectionGridViewModel = require('App/ViewModels/Monitors/MonitoringSeriesSectionGridViewModel');
import MonitoringSeriesSectionGridViewModelMapper = require('App/Mappers/Monitors/MonitoringSeriesSectionGridViewModelMapper');
import MonitoringSeriesSectionSearchFilterDTO = require('App/DTOs/MonitoringSeriesSectionSearchFilterDTO');

class MonitoringSeriesSectionService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getMonitoringSeriesSection(searchFilter: MonitoringSeriesSectionSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.
            concat("/DocumentSeriesService.GetMonitoringSeriesBySection(dateFrom='", searchFilter.dateFrom,
            "',dateTo='", searchFilter.dateTo, "')");
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let viewModelMapper = new MonitoringSeriesSectionGridViewModelMapper();
                let monitoringSeriesSection: MonitoringSeriesSectionGridViewModel[] = [];
                $.each(response.value, function (i, value) {
                    monitoringSeriesSection.push(viewModelMapper.Map(value));
                });
                callback(monitoringSeriesSection);
            };
        }, error);
    }
}

export = MonitoringSeriesSectionService;