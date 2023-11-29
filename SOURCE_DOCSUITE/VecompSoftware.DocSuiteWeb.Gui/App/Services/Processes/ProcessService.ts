import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ProcessModelMapper = require("App/Mappers/Processes/ProcessModelMapper");
import ProcessModel = require("App/Models/Processes/ProcessModel");
import PaginationModel = require("App/Models/Commons/PaginationModel");
import ODATAResponseModel = require("App/Models/ODATAResponseModel");

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

    getAvailableProcesses(name: string, loadOnlyMy: boolean, categoryId: number, dossierId: string, isProcessActive: boolean, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        if (name) {
            name = `'${name}'`;
        }
        let url: string = `${this._configuration.ODATAUrl}/ProcessService.AvailableProcesses(name=${name},categoryId=${!!categoryId ? categoryId.toString() : null},dossierId=${!!dossierId ? dossierId.toString() : null},loadOnlyMy=${loadOnlyMy},isProcessActive=${isProcessActive})?$orderby=Name asc`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback && response) {
                    let mapper = new ProcessModelMapper();
                    const processes: ProcessModel[] = mapper.MapCollection(response.value);

                    callback(processes);
                }
            }, error);
    }

    getProcessesByCategoryId(categoryId: number, callback?: (data: ProcessModel[] | ODATAResponseModel<ProcessModel>) => any, error?: (exception: ExceptionDTO) => any, paginationModel?: PaginationModel): void {
        let url: string = this._configuration.ODATAUrl;
        let odataFilter: string = `$filter=Category/EntityShortId eq ${categoryId} and EndDate eq null&$expand=Dossier,Category,Roles&$orderby=Name`;

        if (paginationModel) {
            odataFilter = `${odataFilter}&$skip=${paginationModel.Skip}&$top=${paginationModel.Take}&$count=true`;
        }

        this.getRequest(url, odataFilter,
            (response: any) => {
                if (!callback) {
                    return;
                }

                let processes: ProcessModel[] = [];

                if (response && response.value) {
                    let mapper = new ProcessModelMapper();
                    processes = mapper.MapCollection(response.value);
                }

                if (!paginationModel) {
                    callback(processes);
                    return;
                }


                const odataResult: ODATAResponseModel<ProcessModel> = new ODATAResponseModel<ProcessModel>(response);
                odataResult.value = processes;
                callback(odataResult);
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

    countCategoryProcesses(categoryId: number, loadOnlyMy: boolean, callback?: (processesCount: number) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataURL: string = this._configuration.ODATAUrl;
        let odataQuery: string = `${odataURL}/ProcessService.CountCategoryProcesses(categoryId=${categoryId},loadOnlyMy=${loadOnlyMy})`;

        this.getRequest(odataQuery, null,
            (response: any) => {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
    }

    getCategoryProcesses(categoryId: number, loadOnlyMy: boolean, paginationModel: PaginationModel, callback?: (categoryProcesses: ProcessModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataURL: string = this._configuration.ODATAUrl;
        let odataQuery: string = `${odataURL}/ProcessService.CategoryProcesses(categoryId=${categoryId},loadOnlyMy=${loadOnlyMy},skip=${paginationModel.Skip},top=${paginationModel.Take})`;

        this.getRequest(odataQuery, null,
            (response: any) => {
                if (callback && response) {
                    let mapper = new ProcessModelMapper();
                    const processes: ProcessModel[] = mapper.MapCollection(response.value);

                    callback(processes);
                }
            }, error);
    }

    findProcessesByName(processName: string, loadOnlyActiveProcesses: boolean, callback?: (categoryProcesses: ProcessModel[]) => any, error?: (exception: ExceptionDTO) => any, tenantAOOId?: string, includeProperties: string[] = []): void {
        let baseOdataURL: string = this._configuration.ODATAUrl;

        const filterQueries: string[] = [];
        if (processName) {
            filterQueries.push(`contains(Name, '${processName}')`);
        }

        if (loadOnlyActiveProcesses) {
            filterQueries.push("(EndDate ge now() or EndDate eq null)");
        }

        if (tenantAOOId) {
            filterQueries.push(`Category/TenantAOO/UniqueId eq ${tenantAOOId}`);
        }

        let odataQuery: string =
            filterQueries.length
                ? `${baseOdataURL}?$filter=${filterQueries.join(" and ")}&$orderby=Name`
                : `${baseOdataURL}?$orderby=Name`;

        if (includeProperties && includeProperties.length) {
            odataQuery = `${odataQuery}&$expand=${includeProperties.join(",")}`;
        }

        this.getRequest(odataQuery, null,
            (response: any) => {
                if (!callback) {
                    return;
                }

                let processes: ProcessModel[] = [];
                if (response && response.value) {
                    let mapper = new ProcessModelMapper();
                    processes = mapper.MapCollection(response.value);
                }

                callback(processes);
            }, error);
    }
}

export = ProcessService;