import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import DossierLogViewModelMapper = require('App/Mappers/Dossiers/DossierLogViewModelMapper');
import DossierLogViewModel = require('App/ViewModels/Dossiers/DossierLogViewModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class DossierLogService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
    * Costruttore 
    */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getDossierLogs(idDossier: string, skip: number, top: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$filter=Entity/UniqueId eq ", idDossier, " and (LogType ne VecompSoftware.DocSuiteWeb.Entity.Dossiers.DossierLogType'FolderHystory')&$orderby=RegistrationDate&$skip=", skip.toString(), "&$top=", top.toString(), "&$count=true");
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {

                let mapper = new DossierLogViewModelMapper();
                let dossierLogs: DossierLogViewModel[] = [];
                let responseModel: ODATAResponseModel<DossierLogViewModel> = new ODATAResponseModel<DossierLogViewModel>(response);
                if (response) {
                    $.each(response.value, function (i, value) {
                        dossierLogs.push(mapper.Map(value));
                    });
                    responseModel.value = dossierLogs;

                    callback(responseModel);
                }
            }
        }, error);
    }

} export = DossierLogService;