import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import FascicleSummaryFolderViewModel = require('App/ViewModels/Fascicles/FascicleSummaryFolderViewModel');
import UpdateActionType = require("App/Models/UpdateActionType");
import InsertActionType = require("App/Models/InsertActionType");
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleSummaryFolderViewModelMapper = require('App/Mappers/Fascicles/FascicleSummaryFolderViewModelMapper');

class FascicleFolderService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
    * Costruttore 
    */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getById(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=UniqueId eq ${uniqueId}`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
    }

    getDefaultFascicleFolder(idfascicle: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=Fascicle/UniqueId eq ${idfascicle} and Name eq 'Fascicolo' and FascicleFolderLevel eq 2`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
    }

    getChildren(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/FascicleFolderService.GetChildrenByParent(idFascicleFolder=", uniqueId, ")")
        let data: string = "$orderby=Name asc";
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let mapper = new FascicleSummaryFolderViewModelMapper();
                    let dossierFolders: FascicleSummaryFolderViewModel[] = [];
                    if (response) {
                        dossierFolders = mapper.MapCollection(response.value);

                        callback(dossierFolders)
                    }
                }
            }, error);
    }

    getByCategoryAndFascicle(idFascicle: string, idCategory: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/FascicleFolderService.GetByCategoryAndFascicle(idFascicle=", idFascicle, ",idCategory=", idCategory.toString(), ")")
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
    }
    /**
* Cancellazione di una FascicleFolder esistente
* @param model
* @param callback
* @param error
*/
    deleteFascicleFolder(model: FascicleFolderModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }

    /**
* Inserisco una nuova FascicleFolder
*/
    insertFascicleFolder(fascicleFolder: FascicleFolderModel, insertAction?: InsertActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        if (insertAction) {
            url = url.concat("?actionType=", insertAction.toString())
        }
        this.postRequest(url, JSON.stringify(fascicleFolder), callback, error);
    }


    /**
    * Aggiorno una FascicleFolder
    */
    updateFascicleFolder(fascicleFolder: FascicleFolderModel, updateAction?: UpdateActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        if (updateAction) {
            url = url.concat("?actionType=", updateAction.toString())
        }
        this.putRequest(url, JSON.stringify(fascicleFolder), callback, error);
    }
} export = FascicleFolderService;