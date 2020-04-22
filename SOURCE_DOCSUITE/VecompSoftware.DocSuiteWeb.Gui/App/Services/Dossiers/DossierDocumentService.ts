import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import DossierSummaryDocumentViewModelMapper = require('App/Mappers/Dossiers/DossierSummaryDocumentViewModelMapper');
import DossierDocumentModel = require('App/Models/Dossiers/DossierDocumentModel');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');


class DossierDocumentService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
    * Costruttore 
    */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getDossierDocuments(idDossier: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$filter=Dossier/UniqueId eq ", idDossier);
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {

                let instance = new DossierSummaryDocumentViewModelMapper();
                let dossierDocuments: BaseEntityViewModel[] = [];
                if (response) {
                    $.each(response.value, function (i, value) {
                        dossierDocuments.push(instance.Map(value));
                    });
                    callback(dossierDocuments);
                }
            }
        }, error);
    }

    insertDossierDocument(model: DossierDocumentModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

} export = DossierDocumentService;