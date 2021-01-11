import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ProcessFascicleTemplateModelMapper = require("App/Mappers/Processes/ProcessFascicleTemplateModelMapper");
import ProcessFascicleTemplateModel = require("App/Models/Processes/ProcessFascicleTemplateModel");

class ProcessFascicleTemplateService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getAll(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper: ProcessFascicleTemplateModelMapper = new ProcessFascicleTemplateModelMapper();
                let processFascicleTemplates: ProcessFascicleTemplateModel[] = [];
                for (let value of response.value) {
                    processFascicleTemplates.push(modelMapper.Map(value));
                }
                callback(processFascicleTemplates);
            }
        }, error);
    }

    getById(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$filter=UniqueId eq ${uniqueId}`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper: ProcessFascicleTemplateModelMapper = new ProcessFascicleTemplateModelMapper();
                let processFascicleTemplate: ProcessFascicleTemplateModel = modelMapper.Map(response.value[0]);
                callback(processFascicleTemplate);
            }
        }, error);
    }

    getFascicleTemplateByDossierFolderId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `?$expand=DossierFolder&$filter=DossierFolder/UniqueId eq ${uniqueId}`;
        this.getRequest(url, data, (response: any) => {
            if (callback) callback(response.value);
        }, error);
    }

    insert(processFascicleTemplate: ProcessFascicleTemplateModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(processFascicleTemplate), callback, error);
    }

    update(processFascicleTemplate: ProcessFascicleTemplateModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(processFascicleTemplate), callback, error);
    }

    delete(processFascicleTemplate: ProcessFascicleTemplateModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(processFascicleTemplate), callback, error);
    }
}

export = ProcessFascicleTemplateService;