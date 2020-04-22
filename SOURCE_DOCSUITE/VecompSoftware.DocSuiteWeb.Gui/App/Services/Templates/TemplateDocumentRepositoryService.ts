import TemplateDocumentRepositoryModel = require('App/Models/Templates/TemplateDocumentRepositoryModel');
import TemplateDocumentFinderViewModel = require('App/ViewModels/Templates/TemplateDocumentFinderViewModel');
import TemplateDocumentRepositoryStatus = require('App/Models/Templates/TemplateDocumentRepositoryStatus');
import TemplateDocumentRepositoryModelMapper = require('App/Mappers/Templates/TemplateDocumentRepositoryModelMapper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');


class TemplateDocumentRepositoryService extends BaseService {
    _configuration: ServiceConfiguration;
    _modelMapper: TemplateDocumentRepositoryModelMapper;

    /**
     * Costruttore
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
        this._modelMapper = new TemplateDocumentRepositoryModelMapper();
    }

    /**
     * Inserisce un nuovo TemplateDocument
     * @param model
     * @param callback
     * @param error
     */
    insertTemplateDocument(model: TemplateDocumentRepositoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Modifica un TemplateDocument
     * @param model
     * @param callback
     * @param error
     */
    updateTemplateDocument(model: TemplateDocumentRepositoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Recupera la lista dei Tag di tutti i Template inseriti
     * @param callback
     * @param error
     */
    getTags(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/TemplateDocumentRepositoryService.GetTags()");
        this.getRequest(url, undefined, callback, error);
    }

    /**
     * Recupera uno specifico Template dato uno specifico ID
     * @param templateId
     * @param callback
     * @param error
     */
    getTemplateById(templateId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$filter=UniqueId eq ", templateId);
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                callback(this._modelMapper.Map(response.value[0]));
            }
        }, error);
    }

    /**
    * Carica i TemplateDocument già esistenti attraverso filtro di ricerca sul nome del template
    * @param name
    * @param model
    * @param callback
    * @param error
    */
    findTemplateDocument(templateFinder: TemplateDocumentFinderViewModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let params: string = '';
        if (templateFinder.hasFilter()) {
            params = params.concat("$filter=");
            let filters: string[] = new Array<string>();
            if (!!templateFinder.Name) {
                filters.push("contains(Name,'".concat(templateFinder.Name, "')"));
            }

            if (templateFinder.Tags.length > 0) {
                let odataTags: string[] = new Array<string>();
                $.each(templateFinder.Tags, (index: number, tag: string) => {
                    odataTags.push("contains(QualityTag,'".concat(tag, "')"));
                });
                filters.push("(".concat(odataTags.join(" or "), ")"));
            }

            if (templateFinder.Status.length > 0) {
                let odataStatus: string[] = new Array<string>();
                $.each(templateFinder.Status, (index: number, status: TemplateDocumentRepositoryStatus) => {
                    odataStatus.push("Status eq VecompSoftware.DocSuiteWeb.Entity.Templates.TemplateDocumentationRepositoryType'".concat(status.toString(), "'"));
                });
                filters.push("(".concat(odataStatus.join(" or "), ")"));
            }
            params = params.concat(filters.join(" and "));
        }

        if (!!params) {
            params = params.concat('&');
        }
        params = params.concat('$orderby=Name asc');
        
        this.getRequest(url, params, (response: any) => {
            if (callback) {
                let tmp: TemplateDocumentRepositoryModel[] = new Array<TemplateDocumentRepositoryModel>();
                $.each(response.value, (index: number, item: any) => {
                    tmp.push(this._modelMapper.Map(item));
                });
                callback(tmp);
            }
        }, error);
    }

    /**
     * Elimina un TemplateDocument esistente
     * @param model
     * @param callback
     * @param error
     */
    deleteTemplateDocument(model: TemplateDocumentRepositoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }
}

export = TemplateDocumentRepositoryService;