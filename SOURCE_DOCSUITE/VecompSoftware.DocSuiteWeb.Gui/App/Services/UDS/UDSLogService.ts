import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import UDSLogViewModelMapper = require('App/Mappers/UDS/UDSLogViewModelMapper');
import UDSLogViewModel = require('App/ViewModels/UDS/UDSLogViewModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class UDSLogService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
    * Costruttore 
    */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getUDSLogs(idUDS: string, skip: number, top: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$filter=IdUDS eq ", idUDS, "&$orderby=RegistrationDate desc&$skip=", skip.toString(), "&$top=", top.toString(), "&$count=true");
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {

                let mapper = new UDSLogViewModelMapper();
                let UDSLogs: UDSLogViewModel[] = [];
                let responseModel: ODATAResponseModel<UDSLogViewModel> = new ODATAResponseModel<UDSLogViewModel>(response);
                if (response) {
                    $.each(response.value, function (i, value) {
                        UDSLogs.push(mapper.Map(value));
                    });
                    responseModel.value = UDSLogs;

                    callback(responseModel);
                }
            }
        }, error);
    }


    getUDSLogsByRegistrationUserAndLogType(registrationUser: string, logType: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$filter=contains(RegistrationUser,'", registrationUser, "') and LogType eq '", logType, "' &$count=true");
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {

                let mapper = new UDSLogViewModelMapper();
                let UDSLogs: UDSLogViewModel[] = [];
                let responseModel: ODATAResponseModel<UDSLogViewModel> = new ODATAResponseModel<UDSLogViewModel>(response);
                if (response) {
                    $.each(response.value, function (i, value) {
                        UDSLogs.push(mapper.Map(value));
                    });
                    responseModel.value = UDSLogs;

                    callback(responseModel);
                }
            }
        }, error);
    }

    getMyUDSLog(idUDS: string, skip: number, top: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/UDSLogService.GetMyLogs(idUDS=", idUDS, ",skip=", skip.toString(), ",top=", top.toString(), ")?$count=true");
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {

                let mapper = new UDSLogViewModelMapper();
                let UDSLogs: UDSLogViewModel[] = [];
                let responseModel: ODATAResponseModel<UDSLogViewModel> = new ODATAResponseModel<UDSLogViewModel>(response);
                if (response) {
                    $.each(response.value, function (i, value) {
                        UDSLogs.push(mapper.Map(value));
                    });
                    responseModel.value = UDSLogs;

                    callback(responseModel);
                }
            }
        }, error);
    }

} export = UDSLogService;