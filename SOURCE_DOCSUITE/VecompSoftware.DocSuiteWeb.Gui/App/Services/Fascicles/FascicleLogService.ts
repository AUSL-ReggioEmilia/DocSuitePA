import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import FascicleLogViewModelMapper = require('App/Mappers/Fascicles/FascicleLogViewModelMapper');
import FascicleLogViewModel = require('App/ViewModels/Fascicles/FascicleLogViewModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleLogType = require('App/Models/Fascicles/FascicleLogType');

class FascicleLogService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
    * Costruttore 
    */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getFascicleLogs(idFascicle: string, skip: number, top: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$filter=Entity/UniqueId eq ", idFascicle, "&$orderby=RegistrationDate&$skip=", skip.toString(), "&$top=", top.toString(), "&$count=true");
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {

                let mapper = new FascicleLogViewModelMapper();
                let fascicleLogs: FascicleLogViewModel[] = [];
                let responseModel: ODATAResponseModel<FascicleLogViewModel> = new ODATAResponseModel<FascicleLogViewModel>(response);
                if (response) {
                    $.each(response.value, function (i, value) {
                        fascicleLogs.push(mapper.Map(value));
                    });
                    responseModel.value = fascicleLogs;

                    callback(responseModel);
                }
            }
        }, error);
    }
    
    getDeletedFasciclesByFascicleId(uniqueId: string, fascicleLogType: FascicleLogType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `?$filter=LogType eq '${FascicleLogType[fascicleLogType]}' and Entity/UniqueId eq ${uniqueId} &$orderby=RegistrationDate desc`;
        url = url.concat(data);
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                let mapper = new FascicleLogViewModelMapper();
                let fascicleLogs: FascicleLogViewModel[] = [];
                let responseModel: ODATAResponseModel<FascicleLogViewModel> = new ODATAResponseModel<FascicleLogViewModel>(response);
                if (response) {
                    $.each(response.value, function (i, value) {
                        fascicleLogs.push(mapper.Map(value));
                    });
                    responseModel.value = fascicleLogs;

                    callback(responseModel.value);
                }
            }
        }, error);
    }

    countByFascicleId(uniqueId: string, fascicleLogType: FascicleLogType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void  {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=LogType eq '${FascicleLogType[fascicleLogType]}' and Entity/UniqueId eq ${uniqueId}`; 
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

} export = FascicleLogService;