import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import TemplateReportModel = require('App/Models/Templates/TemplateReportModel');
import TemplateReportStatus = require('App/Models/Templates/TemplateReportStatus');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');


class TemplateReportService extends BaseService {
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
        let qs: string = "$filter=UniqueId eq ".concat(templateId);
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
    }

    find(name: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string;
        if (name) {
            qs = "$filter=contains(Name, '".concat(name, "')");
        }        
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    /**
     * Inserisce un nuovo report template
     * @param model
     * @param callback
     * @param error
     */
    insertTemplateReport(model: TemplateReportModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Modifica un report template esistente
     * @param model
     * @param callback
     * @param error
     */
    updateTemplateReport(model: TemplateReportModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Cancellazione di un report template esistente
     * @param model
     * @param callback
     * @param error
     */
    deleteTemplateReport(model: TemplateReportModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }
}

export = TemplateReportService;