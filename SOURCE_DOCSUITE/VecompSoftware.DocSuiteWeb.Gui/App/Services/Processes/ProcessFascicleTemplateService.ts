import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ProcessFascicleTemplateModelMapper = require("App/Mappers/Processes/ProcessFascicleTemplateModelMapper");
import ProcessFascicleTemplateModel = require("App/Models/Processes/ProcessFascicleTemplateModel");
import PaginationModel = require("App/Models/Commons/PaginationModel");
import ODATAResponseModel = require("App/Models/ODATAResponseModel");

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
        let url: string = `${this._configuration.ODATAUrl}?$filter=UniqueId eq ${uniqueId}&$expand=Process($expand=Category)`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper: ProcessFascicleTemplateModelMapper = new ProcessFascicleTemplateModelMapper();
                let processFascicleTemplate: ProcessFascicleTemplateModel = modelMapper.Map(response.value[0]);
                callback(processFascicleTemplate);
            }
        }, error);
    }

    countFascicleTemplatesByDossierFolderId(dossierFolderId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataUrl: string = this._configuration.ODATAUrl;
        let odataQuery = `${odataUrl}/$count?$filter=DossierFolder/UniqueId eq ${dossierFolderId}`;

        this.getRequest(odataQuery, null, (response: any) => {
            if (callback) {
                callback(response);
            }
        }, error);
    }

    getFascicleTemplateByDossierFolderId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, paginationModel?: PaginationModel): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=DossierFolder/UniqueId eq ${uniqueId}`;

        if (paginationModel) {
            data = `${data}&$skip=${paginationModel.Skip}&$top=${paginationModel.Take}&$count=true`;
        }

        this.getRequest(url, data, (response: any) => {
            if (!callback) {
                return;
            }

            let fascicleTemplates: ProcessFascicleTemplateModel[] = [];

            if (response && response.value) {
                let mapper: ProcessFascicleTemplateModelMapper = new ProcessFascicleTemplateModelMapper();
                fascicleTemplates = mapper.MapCollection(response.value);
            }

            if (!paginationModel) {
                callback(fascicleTemplates);
                return;
            }

            const odataResult: ODATAResponseModel<ProcessFascicleTemplateModel> = new ODATAResponseModel<ProcessFascicleTemplateModel>(response);
            odataResult.value = fascicleTemplates;
            callback(odataResult);
        }, error);
    }

    getActiveFascicleTemplatesByDossierFolderId(dossierFolderId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let odataQuery: string = `$filter=DossierFolder/UniqueId eq ${dossierFolderId} and EndDate eq null&$orderby=Name`;

        this.getRequest(url, odataQuery, (response: any) => {
            if (!callback) {
                return;
            }

            let fascicleTemplates: ProcessFascicleTemplateModel[] = [];
            if (response && response.value) {
                let mapper: ProcessFascicleTemplateModelMapper = new ProcessFascicleTemplateModelMapper();
                fascicleTemplates = mapper.MapCollection(response.value);
            }

            callback(fascicleTemplates);
        }, error)
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