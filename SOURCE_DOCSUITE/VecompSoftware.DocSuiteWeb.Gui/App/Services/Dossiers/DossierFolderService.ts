import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import DossierSummaryFolderViewModelMapper = require('App/Mappers/Dossiers/DossierSummaryFolderViewModelMapper');
import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import UpdateActionType = require("App/Models/UpdateActionType");
import InsertActionType = require("App/Models/InsertActionType");
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import IDossierFolderService = require('App/Services/Dossiers/IDossierFolderService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ProcessFascicleTemplateModelMapper = require('../../Mappers/Processes/ProcessFascicleTemplateModelMapper');
import ProcessFascicleTemplateModel = require('../../Models/Processes/ProcessFascicleTemplateModel');
import DossierFolderModelMapper = require('../../Mappers/Dossiers/DossierFolderModelMapper');
import DossierModelMapper = require('App/Mappers/Dossiers/DossierModelMapper');


class DossierFolderService extends BaseService implements IDossierFolderService {
    private _configuration: ServiceConfiguration;

    /**
    * Costruttore 
    */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getChildren(uniqueId: string, status: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/DossierFolderService.GetChildrenByParent(idDossierFolder=", uniqueId, ",status=", status.toString(), ")")
        let data: string = "$orderby=Name asc";

        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let mapper = new DossierSummaryFolderViewModelMapper();
                    let dossierFolders: DossierSummaryFolderViewModel[] = [];
                    if (response) {
                        dossierFolders = mapper.MapCollection(response.value);
                        callback(dossierFolders)
                    }
                }
            }, error);
    }

    getProcessFascicleChildren(uniqueId: string, status: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let baseUrl: string = this._configuration.ODATAUrl;
        let url = `${baseUrl}/DossierFolderService.GetChildrenByParent(idDossierFolder=${uniqueId},status=${status.toString()})`;
        let data: string = "$orderby=Name asc&$filter=Status in ('InProgress','Folder')";

        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let mapper = new DossierSummaryFolderViewModelMapper();
                    let dossierFolders: DossierSummaryFolderViewModel[] = [];
                    if (response) {
                        dossierFolders = mapper.MapCollection(response.value);
                        callback(dossierFolders)
                    }
                }
            }, error);
    }

    /**
    * Inserisco una nuova DossierFolder
    */
    insertDossierFolder(dossierFolder: DossierFolderModel, insertAction?: InsertActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        if (insertAction) {
            url = url.concat("?actionType=", insertAction.toString())
        }
        this.postRequest(url, JSON.stringify(dossierFolder), callback, error);
    }

    /**
    * Cancellazione di una DossierFolder esistente
    * @param model
    * @param callback
    * @param error
    */
    deleteDossierFolder(model: DossierFolderModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }

    /**
    * Aggiorno una DossierFolder
    */
    updateDossierFolder(dossierFolder: DossierFolderModel, updateAction?: UpdateActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        if (updateAction) {
            url = url.concat("?actionType=", updateAction.toString())
        }
        this.putRequest(url, JSON.stringify(dossierFolder), callback, error);
    }

    getDossierFolder(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=UniqueId eq ".concat(uniqueId, "&$expand=Category,Fascicle,DossierFolderRoles($expand=Role)");
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let instance = new DossierSummaryFolderViewModelMapper();
                    if (response) {
                        callback(instance.Map(response.value[0]))
                    }
                }
            }, error);
    }

    getFullDossierFolder(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=UniqueId eq ".concat(uniqueId, "&$expand=Fascicle,Category,Dossier($expand=MetadataRepository),DossierFolderRoles($expand=Role)");
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    callback(response.value[0]);
                }
            }, error);
    }

    getFascicleTemplatesByDossierFolderId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$expand=FascicleTemplates&$filter=UniqueId eq ${uniqueId}&$select=FascicleTemplates&$orderby=Name`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    let mapper: ProcessFascicleTemplateModelMapper = new ProcessFascicleTemplateModelMapper();
                    let results: ProcessFascicleTemplateModel[] = [];
                    if (response.value.length) {
                        for (let item of response.value[0].FascicleTemplates) {
                            results.push(mapper.Map(item));
                        }
                    }
                    callback(results);
                }
            }, error);
    }

    getProcessFolders(name: string, idProcess: string, loadOnlyActive: boolean, loadOnlyMy: boolean, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        if (name) {
            name = `'${name}'`;
        }
        let url: string = `${this._configuration.ODATAUrl}/DossierFolderService.GetProcessFolders(name=${name},idProcess=${idProcess},loadOnlyActive=${loadOnlyActive},loadOnlyMy=${loadOnlyMy})?$filter=Status ne 'Fascicle'&$orderby=Name`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
    }

    getDossierFolderById(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$filter=UniqueId eq ${uniqueId}&$expand=Dossier,DossierFolderRoles($expand=Role($expand=Father)),Category`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback && response) {
                    let mapper: DossierFolderModelMapper = new DossierFolderModelMapper();
                    let dossierMapper: DossierModelMapper = new DossierModelMapper();
                    let dossierFolderModels: DossierFolderModel[] = mapper.MapCollection(response.value);
                    dossierFolderModels.forEach((dossierFolderModel => dossierFolderModel.Dossier = dossierMapper.Map(dossierFolderModel.Dossier)));
                    callback(dossierFolderModels);
                }
            }, error);
    }

    checkIfDossierFolderFascicleIsActive(dossierFolderId: string, callback?: (fascicleStatus: boolean) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$filter=UniqueId eq ${dossierFolderId}&$expand=Fascicle`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback && response) {
                    let mapper: DossierFolderModelMapper = new DossierFolderModelMapper();
                    let dossierFolderModel: DossierFolderModel = mapper.Map(response.value[0]);
                    let dossierFolderFascicleIsActive: boolean = !dossierFolderModel.Fascicle.EndDate;
                    callback(dossierFolderFascicleIsActive);
                }
            }, error);
    }

    getAllParentsOfFascicle(idDossier: string, idFascicle: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}/DossierFolderService.GetAllParentsOfFascicle(idDossier=${idDossier}, idFascicle=${idFascicle})`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    let mapper = new DossierSummaryFolderViewModelMapper();
                    let dossierFolders: DossierSummaryFolderViewModel[] = [];
                    if (response) {
                        dossierFolders = mapper.MapCollection(response.value);
                        callback(dossierFolders)
                    }
                }
            }, error);
    }

    getLinkedDossierByFascicleId(idFascicle: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$expand=Category,Fascicle,Dossier($expand=Container)&$filter=Fascicle/uniqueid eq ${idFascicle}`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    hasAssociatedFascicles(idDossier: string, callback?: (data: boolean) => void, error?: (exception: ExceptionDTO) => void): void {
        let url: string = `${this._configuration.ODATAUrl}/DossierFolderService.HasAssociatedFascicles(idDossier=${idDossier})`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    getProcessByFascicleId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=Dossier/DossierType eq 'Process' and Fascicle/UniqueId eq ${uniqueId}&$expand=Dossier($expand=DossierFolders)`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    getByFascicleId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=Dossier/DossierType ne 'Process' and Fascicle/UniqueId eq ${uniqueId}&$expand=Dossier`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    countByFascicleId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=Dossier/DossierType ne 'Process' and Fascicle/UniqueId eq ${uniqueId}`;
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }
} export = DossierFolderService;