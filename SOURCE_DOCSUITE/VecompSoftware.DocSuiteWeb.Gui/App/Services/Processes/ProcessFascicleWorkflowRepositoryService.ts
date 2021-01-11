import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ProcessFascicleWorkflowRepositoryModelMapper = require("App/Mappers/Processes/ProcessFascicleWorkflowRepositoryModelMapper");
import ProcessFascicleWorkflowRepositoryModel = require("App/Models/Processes/ProcessFascicleWorkflowRepositoryModel");

class ProcessFascicleWorkflowRepositoryService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getAll(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper: ProcessFascicleWorkflowRepositoryModelMapper = new ProcessFascicleWorkflowRepositoryModelMapper();
                let processFascicleWorkflowRepositories: ProcessFascicleWorkflowRepositoryModel[] = [];
                for (let value of response.value) {
                    processFascicleWorkflowRepositories.push(modelMapper.Map(value));
                }
                callback(processFascicleWorkflowRepositories);
            }
        }, error);
    }

    getById(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$filter=UniqueId eq ${uniqueId}`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper: ProcessFascicleWorkflowRepositoryModelMapper = new ProcessFascicleWorkflowRepositoryModelMapper();
                let processFascicleWorkflowRepository: ProcessFascicleWorkflowRepositoryModel = modelMapper.Map(response.value[0]);
                callback(processFascicleWorkflowRepository);
            }
        }, error);
    }

    insert(processFascicleWorkflowRepository: ProcessFascicleWorkflowRepositoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(processFascicleWorkflowRepository), callback, error);
    }

    update(processFascicleWorkflowRepository: ProcessFascicleWorkflowRepositoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(processFascicleWorkflowRepository), callback, error);
    }

    delete(processFascicleWorkflowRepository: ProcessFascicleWorkflowRepositoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(processFascicleWorkflowRepository), callback, error);
    }

    getByDossierFolderId(processId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$filter=DossierFolder/UniqueId eq ${processId}&$expand=Process,WorkflowRepository,DossierFolder($expand=DossierFolderRoles($expand=Role))`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper: ProcessFascicleWorkflowRepositoryModelMapper = new ProcessFascicleWorkflowRepositoryModelMapper();
                let processFascicleWorkflowRepositories: ProcessFascicleWorkflowRepositoryModel[] = [];
                for (let value of response.value) {
                    processFascicleWorkflowRepositories.push(modelMapper.Map(value));
                }
                callback(processFascicleWorkflowRepositories);
            }
        }, error);
    }
}

export = ProcessFascicleWorkflowRepositoryService;