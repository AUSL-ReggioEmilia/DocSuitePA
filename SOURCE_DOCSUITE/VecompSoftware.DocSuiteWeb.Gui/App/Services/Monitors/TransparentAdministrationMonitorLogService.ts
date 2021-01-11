import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import TransparentAdministrationMonitorLogGridViewModel = require('App/ViewModels/Monitors/TransparentAdministrationMonitorLogGridViewModel');
import TransparentAdministrationMonitorLogGridViewModelMapper = require('App/Mappers/Monitors/TransparentAdministrationMonitorLogGridViewModelMapper');
import TransparentAdministrationMonitorLogSearchFilterDTO = require('App/DTOs/TransparentAdministrationMonitorLogSearchFilterDTO');
import TransparentAdministrationMonitorLogModel = require('App/Models/Monitors/TransparentAdministrationMonitorlogModel');

class TransparentAdministrationMonitorLogService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getTransparentAdministrationMonitorLogs(searchFilter: TransparentAdministrationMonitorLogSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let filters: string = `,userName='${searchFilter.username}'`;
        if (searchFilter.container === "") {
            searchFilter.container = "null";
        }
        filters = filters.concat(`,idContainer=${searchFilter.container}`);
        if (searchFilter.documentType === "") {
            searchFilter.documentType = "null";
        }
        filters = filters.concat(`,environment=${searchFilter.documentType}`);

        let oDataFilters: string = `$orderby=RegistrationDate desc`;
        if (searchFilter.idRole) {
            oDataFilters = oDataFilters.concat(`&$filter=IdRole eq `, searchFilter.idRole.toString())
        }

        let url: string = this._configuration.ODATAUrl.
            concat("/TransparentAdministrationMonitorLogService.GetByDateInterval(dateFrom='", searchFilter.dateFrom,
            "',dateTo='", searchFilter.dateTo, "'", filters, ")?", oDataFilters);
        
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let viewModelmapper = new TransparentAdministrationMonitorLogGridViewModelMapper();
                let transparentAdministrationMonitorLogs: TransparentAdministrationMonitorLogGridViewModel[] = [];
                $.each(response.value, function (i, value) {
                    transparentAdministrationMonitorLogs.push(viewModelmapper.Map(value));
                });
                callback(transparentAdministrationMonitorLogs);
            };
        }, error);
    }

    insertTransparentAdministrationMonitorLog(model: TransparentAdministrationMonitorLogModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }


    updateTransparentAdministrationMonitorLog(model: TransparentAdministrationMonitorLogModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }


    getLatestMonitorLogByDocumentUnit(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=DocumentUnit/UniqueId eq ".concat(documentUnitId, "&$expand=DocumentUnit,Role&$orderby=RegistrationDate desc&$top=1");
        this.getRequest(url, qs, (response: any) => {
            if (callback) {
                if (response && response.value) {
                    callback(response.value[0])
                } else {
                    callback(undefined);
                }
            };
        }, error);
    }
}

export = TransparentAdministrationMonitorLogService;