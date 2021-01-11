import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ProcessModelMapper = require("App/Mappers/Processes/ProcessModelMapper");
import ProcessModel = require("App/Models/Processes/ProcessModel");

class ProcessService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getAll(searchName: string, isActive?: boolean, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$expand=Dossier,Category,Roles($expand=Father)`;
        if (isActive !== null) {
            let filter: string = "";
            if (isActive) {
                filter = "&$filter=(EndDate ge now() or EndDate eq null)";
            }
            else {
                filter = "&$filter=EndDate le now()";
            }
            url = url.concat(filter);
            if (searchName !== "") {
                url = url.concat(` and contains(Name,'${searchName}')`);
            }
        }
        else if (searchName !== "") {
            url = url.concat(`&$filter=contains(Name,'${searchName}')`);
        }
        url = url.concat("&$orderby=Name");
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper: ProcessModelMapper = new ProcessModelMapper();
                let processes: ProcessModel[] = [];
                for (let value of response.value) {
                    processes.push(modelMapper.Map(value));
                }
                callback(processes);
            }
        }, error);
    }

    getById(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$filter=UniqueId eq ${uniqueId}&$expand=Category,Dossier,Roles($expand=Father)`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper: ProcessModelMapper = new ProcessModelMapper();
                let process: ProcessModel = modelMapper.Map(response.value[0]);
                callback(process);
            }
        }, error);
    }

    getAvailableProcesses(name: string, loadOnlyMy: boolean, categoryId: number, dossierId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        if (name) {
            name = `'${name}'`;
        }
        let url: string = `${this._configuration.ODATAUrl}/ProcessService.AvailableProcesses(name=${name},categoryId=${!!categoryId ? categoryId.toString() : null},dossierId=${!!dossierId ? dossierId.toString() : null},loadOnlyMy=${loadOnlyMy})?$orderby=Name asc`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback && response) {
                    let mapper = new ProcessModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
    }

    getProcessesByCategoryId(categoryId: number, callback?: (data: ProcessModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let odataFilter: string = `$filter=Category/EntityShortId eq ${categoryId}&$expand=Dossier,Category,Roles&$orderby=Name`;

        this.getRequest(url, odataFilter,
            (response: any) => {
                if (callback && response) {
                    let mapper = new ProcessModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
    }

    getProcessByDossierFolderId(dossierFolderId: string, callback?: (data: ProcessModel) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let odataFilter: string = `$filter=Dossier/DossierFolders/any(df: df/UniqueId eq ${dossierFolderId})`; 

        this.getRequest(url, odataFilter,
            (response: any) => {
                if (callback && response) {
                    let mapper = new ProcessModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
    }

    insert(process: ProcessModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(process), callback, error);
    }

    update(process: ProcessModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(process), callback, error);
    }

    delete(process: ProcessModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(process), callback, error);
    }
}

export = ProcessService;