import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import TemplateCollaborationStatus = require('App/Models/Templates/TemplateCollaborationStatus');
import UpdateActionType = require("App/Models/UpdateActionType");
import ExceptionDTO = require('App/DTOs/ExceptionDTO');


class TemplateCollaborationService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getById(templateId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=UniqueId eq ".concat(templateId, "&$expand=TemplateCollaborationUsers($expand=Role),TemplateCollaborationDocumentRepositories,Roles");
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {                    
                    callback(response.value[0]);
                }
            }, error);
    }

    getTemplates(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=Status eq 'Active'";
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    /**
     * Inserisce un nuovo Template di collaborazione
     * @param model
     * @param callback
     * @param error
     */
    insertTemplateCollaboration(model: TemplateCollaborationModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Modifica un Template di collaborazione esistente
     * @param model
     * @param callback
     * @param error
     */
    updateTemplateCollaboration(model: TemplateCollaborationModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Cancellazione di un Template di collaborazione esistente
     * @param model
     * @param callback
     * @param error
     */
    deleteTemplateCollaboration(model: TemplateCollaborationModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Pubblica un template di collaborazione
     * @param model
     * @param callback
     * @param error
     */
    publishTemplate(model: TemplateCollaborationModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        model.Status = TemplateCollaborationStatus.Active;
        let url: string = this._configuration.WebAPIUrl.concat("?actionType=", UpdateActionType.TemplateCollaborationPublish.toString());
        this.putRequest(url, JSON.stringify(model), callback, error);
    }
}

export = TemplateCollaborationService;