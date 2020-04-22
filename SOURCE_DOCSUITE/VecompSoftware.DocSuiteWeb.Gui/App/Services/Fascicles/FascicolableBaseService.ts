import BaseService = require('App/Services/BaseService');
import IFascicolableBaseService = require('App/Services/Fascicles/IFascicolableBaseService');
import IFascicolableBaseModel = require('App/Models/Fascicles/IFascicolableBaseModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import InsertActionType = require("App/Models/InsertActionType");
import FascicolableActionType = require("App/Models/FascicolableActionType");
import UpdateActionType = require('App/Models/UpdateActionType');

abstract class FascicolableBaseService<T extends IFascicolableBaseModel> extends BaseService implements IFascicolableBaseService<T> {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    abstract getByDocumentUnitAndFascicle(idDocumentUnit: string, idFascicle: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void;

    /**
     * Inserisce una nuova Document Unit in un Fascicolo
     * @param model
     * @param callback
     * @param error
     */
    insertFascicleUD(model: T, insertAction?: FascicolableActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        if (insertAction && insertAction == FascicolableActionType.AutomaticDetection) {
            url = url.concat("?actionType=", InsertActionType.AutomaticIntoFascicleDetection.toString())
        }
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Modifica una Document Unit in un Fascicolo
     * @param model
     * @param callback
     * @param error
     */
    updateFascicleUD(model: T, updateAction?: UpdateActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        if (updateAction) {
            url = url.concat("?actionType=", updateAction.toString())
        }
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Rimuove una Document Unit da un Fascicolo
     * @param model
     * @param callback
     * @param error
     */
    deleteFascicleUD(model: T, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }
}

export = FascicolableBaseService;