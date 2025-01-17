﻿/// <amd-dependency path="../../core/extensions/string" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import ProtocolModel = require('App/Models/Protocols/ProtocolModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import NotLinkedFasciclesModel = require('App/Models/Fascicles/NotLinkedFasciclesModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import UpdateActionType = require("App/Models/UpdateActionType");
import DeleteActionType = require("App/Models/DeleteActionType");
import InsertActionType = require('App/Models/InsertActionType')
import FascicleType = require('App/Models/Fascicles/FascicleType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleModelMapper = require('App/Mappers/Fascicles/FascicleModelMapper');
import IFascicleService = require('App/Services/Fascicles/IFascicleService');
import PaginationModel = require('App/Models/Commons/PaginationModel');

class FascicleService extends BaseService implements IFascicleService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    /**
     * Inserisce un nuovo Fascicolo
     * @param model
     * @param callback
     * @param error
     */
    insertFascicle(model: FascicleModel, actionType?: InsertActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        if (!actionType) {
            switch (model.FascicleType) {
                case FascicleType.Activity:
                    url = url.concat("?actionType=", InsertActionType.InsertActivityFascicle.toString())
                    break;
                case FascicleType.Period:
                    url = url.concat("?actionType=", InsertActionType.InsertPeriodicFascicle.toString())
                    break;
                case FascicleType.Procedure:
                    break;
            }
        } else {
            url = url.concat("?actionType=", actionType.toString())
        }
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Modifica un Fascicolo esistente
     * @param model
     * @param callback
     * @param error
     */
    updateFascicle(model: FascicleModel, actionType?: UpdateActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        if (model.FascicleType == FascicleType.Activity && actionType !== UpdateActionType.ChangeFascicleType) {
            actionType = UpdateActionType.ActivityFascicleUpdate;
        }
        if (actionType) {
            url = url.concat("?actionType=", actionType.toString());
        }
        this.putRequest(url, JSON.stringify(model), callback, error);
    }


    ///**
    //* Cancella un Fascicolo esistente
    //* @param model
    //* @param callback
    //* @param error
    //*/
    deleteFascicle(model: FascicleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl.concat("?actionType=", DeleteActionType.CancelFascicle.toString());
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }


    /**
     * Chiude un Fascicolo
     * @param model
     * @param callback
     * @param error
     */
    closeFascicle(model: FascicleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        if (model.FascicleType.toString() != FascicleType.Activity.toString()) {
            url = url.concat("?actionType=", UpdateActionType.FascicleClose.toString())
        } else {
            url = url.concat("?actionType=", UpdateActionType.ActivityFascicleClose.toString())
        }
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Recupera un Fascicolo per ID
     * @param id
     * @param callback
     * @param error
     */
    getFascicle(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=UniqueId eq ${id}&$expand=Category($expand=CategoryFascicles($expand=CategoryFascicleRights)),Container,Contacts,FascicleDocumentUnits,FascicleDocuments,MetadataRepository,DossierFolders,FascicleRoles($expand=Role($expand=Father))`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let mapper = new FascicleModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
    }

    /**
     * Recupera i Fascicoli disponibili per l'associazione di una Document Unit
     * TODO: da implementare in SignalR
     * @param uniqueId
     * @param environment
     * @param qs
     * @param callback
     * @param error
     */
    getAvailableFascicles(uniqueId: string, name: string, topElement: string, skipElement: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/FascicleService.AvailableFascicles(uniqueId=", uniqueId, ")", "?$orderby=Title,FascicleObject");
        let qs: string = "$count=true&$top=".concat(topElement, "&$skip=", skipElement.toString());
        if (!String.isNullOrEmpty(name)) {

            qs = qs.concat("&$filter=contains(tolower(concat(concat(Title, ' '),FascicleObject)),'", name.toLowerCase().replace(/'/g, "''"), "')");
        }

        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let responseModel: ODATAResponseModel<FascicleModel> = new ODATAResponseModel<FascicleModel>(response);

                    let instances: Array<FascicleModel> = new Array<FascicleModel>();
                    let mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    responseModel.value = instances;

                    callback(responseModel);
                }
            }, error);
    }

    /**
     * TODO: da implementare in SignalR
     * @param uniqueId
     * @param environment
     * @param qs
     * @param callback
     * @param error
     */
    getAssociatedFascicles(uniqueId: string, environment: number, qs: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/FascicleService.DocumentUnitAssociated(uniqueId=", uniqueId, ")");
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let instances: Array<FascicleModel> = new Array<FascicleModel>();
                    let mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    callback(instances);
                }
            }, error);
    }

    /**
     * TODO: da implementare in SignalR
     * @param model
     * @param qs
     * @param callback
     * @param error
     */
    getNotLinkedFascicles(model: NotLinkedFasciclesModel, name: string, topElement: string, skipElement: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/FascicleService.NotLinkedFascicles(idFascicle=" + model.Fascicle.UniqueId + " ,idCategory=" + model.SelectedCategoryId + ")");
        let qs: string = "$count=true&$top=".concat(topElement, "&$skip=", skipElement.toString());
        if (!String.isNullOrEmpty(name)) {
            qs = qs.concat("&$filter=contains(tolower(concat(concat(Title, ' '),FascicleObject)),'", name.toLowerCase(), "')");
        }

        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let responseModel: ODATAResponseModel<FascicleModel> = new ODATAResponseModel<FascicleModel>(response);

                    let instances: Array<FascicleModel> = new Array<FascicleModel>();
                    let mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    responseModel.value = instances;

                    callback(responseModel);
                }
            },
            error
        );
    }
    getFascicleByCategorySkipTop(idCategory: number, name: string, topElement: string, skipElement: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/FascicleService.GetFasciclesByCategory(idCategory=", idCategory.toString(), ",name='", name, "')");
        let qs: string = "$count=true&$top=".concat(topElement, "&$skip=", skipElement.toString());
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {

                    let responseModel: ODATAResponseModel<FascicleModel> = new ODATAResponseModel<FascicleModel>(response);
                    let instances: Array<FascicleModel> = new Array<FascicleModel>();
                    let mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    responseModel.value = instances;

                    callback(responseModel);
                }
            }
        )
    }
    /**
     * TODO: da implementare in SignalR
     * @param model
     * @param qs
     * @param callback
     * @param error
     */
    getLinkedFascicles(model: FascicleModel, qs: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=UniqueId eq ".concat(model.UniqueId, '&$expand=FascicleLinks($expand=FascicleLinked($expand=Category))');
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let mapper = new FascicleModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            },
            error);
    }

    getFascicleByCategory(idCategory: number, name: string, hasProcess?: boolean, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataUrl: string = this._configuration.ODATAUrl;
        let odataQuery = `${odataUrl}/FascicleService.GetFasciclesByCategory(idCategory=${idCategory},name='${name}',hasProcess=${hasProcess})`;

        this.getRequest(odataQuery, null,
            (response: any) => {
                if (callback) {

                    let responseModel: ODATAResponseModel<FascicleModel> = new ODATAResponseModel<FascicleModel>(response);
                    let instances: Array<FascicleModel> = new Array<FascicleModel>();
                    let mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    responseModel.value = instances;

                    callback(responseModel);
                }
            }
        )
    }

    hasInsertRight(fascicleType: FascicleType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`/FascicleService.HasInsertRight(fascicleType=VecompSoftware.DocSuiteWeb.Entity.Fascicles.FascicleType'${FascicleType[fascicleType]}')`);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }

                    callback(response.value);
                }
            }, error);
    }

    hasManageableRight(idFascicle: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`/FascicleService.HasManageableRight(idFascicle=${idFascicle})`);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }

                    callback(response.value);
                }
            }, error);
    }

    hasViewableRight(idFascicle: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`/FascicleService.HasViewableRight(idFascicle=${idFascicle})`);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }

                    callback(response.value);
                }
            }, error);
    }

    isManager(idFascicle: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`/FascicleService.IsManager(idFascicle=${idFascicle})`);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }

                    callback(response.value);
                }
            }, error);
    }

    hasFascicolatedDocumentUnits(idFascicle: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`/FascicleService.HasFascicolatedDocumentUnits(idFascicle=${idFascicle})`);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }

                    callback(response.value);
                }
            }, error);
    }

    getAuthorizedFasciclesFromDocumentUnit(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/FascicleService.AuthorizedFasciclesFromDocumentUnit(uniqueIdDocumentUnit=", uniqueId, ")");
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    let instances: Array<FascicleModel> = new Array<FascicleModel>();
                    let mapper = new FascicleModelMapper();
                    instances = mapper.MapCollection(response.value);
                    callback(instances);
                }
            }, error);
    }

    countAuthorizedFasciclesFromDocumentUnit(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/FascicleService.CountAuthorizedFasciclesFromDocumentUnit(uniqueIdDocumentUnit=", uniqueId, ")");
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }

                    callback(response.value);
                }
            }, error);
    }

    countAuthorizedCategoryFascicles(idCategory: number, callback?: (count: number) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataURL: string = this._configuration.ODATAUrl;
        let odataQuery: string = `${odataURL}/FascicleService.CountAuthorizedCategoryFascicles(idCategory=${idCategory})`;

        this.getRequest(odataQuery, null,
            (response: any) => {
                if (!callback && !response) {
                    return;
                }

                callback(response.value);
            }, error);
    }

    getAuthorizedCategoryFascicles(idCategory: number, paginationModel: PaginationModel, callback?: (data: FascicleModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataURL: string = this._configuration.ODATAUrl;
        let odataQuery: string = `${odataURL}/FascicleService.AuthorizedCategoryFascicles(idCategory=${idCategory},skip=${paginationModel.Skip},top=${paginationModel.Take})`;

        this.getRequest(odataQuery, null,
            (response: any) => {
                if (!callback) {
                    return;
                }

                let mapper = new FascicleModelMapper();
                const fascicles: FascicleModel[] = mapper.MapCollection(response.value);

                callback(fascicles);
            }, error);
    }

    getFasciclesFromDocumentUnit(idDocumentUnit: string, callback?: (data: FascicleModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataURL: string = this._configuration.ODATAUrl;
        let odataQuery: string = `${odataURL}/FascicleService.FasciclesFromDocumentUnit(idDocumentUnit=${idDocumentUnit})`;

        this.getRequest(odataQuery, null,
            (response: any) => {
                if (!callback) {
                    return;
                }

                let mapper = new FascicleModelMapper();
                const fascicles: FascicleModel[] = mapper.MapCollection(response.value);

                callback(fascicles);
            }, error);
    }
}

export = FascicleService;