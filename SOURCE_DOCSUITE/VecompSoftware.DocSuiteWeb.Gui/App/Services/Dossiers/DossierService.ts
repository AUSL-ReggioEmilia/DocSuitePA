import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import DossierSummaryViewModelMapper = require('App/Mappers/Dossiers/DossierSummaryViewModelMapper');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import DossierSummaryContactViewModelMapper = require('App/Mappers/Dossiers/DossierSummaryContactViewModelMapper');
import DossierSummaryRoleViewModelMapper = require('App/Mappers/Dossiers/DossierSummaryRoleViewModelMapper');
import DossierModel = require('App/Models/Dossiers/DossierModel');
import DossierGridViewModelMapper = require('App/Mappers/Dossiers/DossierGridViewModelMapper');
import DossierGridViewModel = require('App/ViewModels/Dossiers/DossierGridViewModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');
import DossierSearchFilterDTO = require("App/DTOs/DossierSearchFilterDTO");
import DossierSummaryViewModel = require('App/ViewModels/Dossiers/DossierSummaryViewModel');
import DossierType = require('App/Models/Dossiers/DossierType');

class DossierService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getAllDossiers(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let viewModelMapper = new DossierSummaryViewModelMapper();
                let dossiers: DossierSummaryViewModel[] = [];
                $.each(response.value, function (i, value) {
                    dossiers.push(viewModelMapper.Map(value));
                });
                callback(dossiers);
            };
        }, error);
    }


    getAuthorizedDossiers(searchFilter: DossierSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataUrl: string = `${this._configuration.ODATAUrl}/DossierService.GetAuthorizedDossiers`;
        let odataActionParameter: string = JSON.stringify({ finder: searchFilter });
        this.postRequest(odataUrl, odataActionParameter, (response: any) => {
            if (callback && response) {
                let viewModelMapper = new DossierGridViewModelMapper();
                let dossiers: DossierGridViewModel[] = [];
                $.each(response.value, function (i, value) {
                    dossiers.push(viewModelMapper.Map(value));
                });
                callback(dossiers);
            };
        }, error);
    }

    countAuthorizedDossiers(searchFilter: DossierSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}/DossierService.CountAuthorizedDossiers`;
        let odataActionParameter: string = JSON.stringify({ finder: searchFilter });
        this.postRequest(url, odataActionParameter, (response: any) => {
            if (callback && response) {
                callback(response.value);
            };
        }, error);
    }

    /**
    * Recupero un DossierModel per ID
    * @param id
    * @param callback
    * @param error
    */
    getDossier(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/DossierService.GetCompleteDossier(uniqueId=", uniqueId, ")");
        let data: string = "";
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    let instance = new DossierSummaryViewModelMapper();
                    callback(instance.Map(response.value[0]));
                }
            }, error);
    }

    getDossierContacts(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/DossierService.GetDossierContacts(uniqueId=", uniqueId, ")");
        let data: string = "";
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    let instance = new DossierSummaryContactViewModelMapper();
                    let dossiercontacts: BaseEntityViewModel[] = [];
                    $.each(response.value, function (i, value) {
                        dossiercontacts.push(instance.Map(value));
                    });
                    callback(dossiercontacts);
                }
            }, error);
    }

    getDossierRoles(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/DossierService.GetDossierRoles(idDossier=", uniqueId, ")");
        let data: string = "";
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    let instance = new DossierSummaryRoleViewModelMapper();
                    callback(instance.MapCollection(response.value));
                }
            }, error);
    }

    isViewableDossier(idDossier: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/DossierService.IsViewableDossier(idDossier=", idDossier, ")");
        let data: string = ""
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
    }

    isManageableDossier(idDossier: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/DossierService.IsManageableDossier(idDossier=", idDossier, ")");
        let data: string = ""
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
    }

    hasRootNode(idDossier: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/DossierService.HasRootNode(idDossier=", idDossier, ")");
        let data: string = ""
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
    }


    /**
 * Inserisce un nuovo Dossier
 * @param model
 * @param callback
 * @param error
 */
    insertDossier(model: DossierModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    /**
* Controlla diritti inserimento dossiers
*/
    hasInsertRight(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/DossierService.HasInsertRight()");
        let data: string = ""
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
    }


    /**
* Modifica un Dossier esistente
* @param model
* @param callback
* @param error
*/
    updateDossier(model: DossierModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    /**
* Controlla diritti modifica del dossier 
*/
    hasModifyRight(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/DossierService.HasModifyRight(idDossier=", uniqueId, ")");
        let data: string = ""
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
    }

    allFasciclesAreClosed(idDossier: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}/DossierService.AllFasciclesAreClosed(idDossier=${idDossier})`;
        let data: string = "";
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
    }

    getDossiersWithTemplatesByFascicleId(idFascicle: string, dossierType: number, onlyFolderHasTemplate: boolean, dossierFolderLevel: number, dossierFolderPath: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=DossierFolders/any(df: df/Fascicle/UniqueId eq ${idFascicle})`;
        let dossierChildren: string = `and DossierFolderLevel eq ${dossierFolderLevel} and startswith(DossierFolderPath,'${dossierFolderPath}');$expand=DossierFolderRoles($expand=Role)`;
        let onlyFolderFilter: string = `($filter=JsonMetadata ne null and Fascicle ne null ${dossierChildren};$expand=Fascicle,DossierFolderRoles($expand=Role);$orderby=Name)`;
        let expandDossierFolder: string = "$expand=DossierRoles($expand=Role),DossierFolders";
        if (dossierType != null && onlyFolderHasTemplate) {
            data = `${data} and DossierType eq '${DossierType[dossierType]}'&${expandDossierFolder}${onlyFolderFilter}`;
        } else if (dossierType != null) {
            data = `${data} and DossierType eq '${DossierType[dossierType]}'&${expandDossierFolder}($filter=DossierFolderLevel eq ${dossierFolderLevel} and startswith(DossierFolderPath,'${dossierFolderPath}');$expand=DossierFolderRoles($expand=Role);$orderby=Name)`;
        } else if (onlyFolderHasTemplate) {
            data = `${data}&${expandDossierFolder}${onlyFolderFilter}`;
        } else {
            data = `${data}&${expandDossierFolder}($filter=Fascicle eq null ${dossierChildren};$orderby=Name)`;
        }

        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
    }
}

export = DossierService; 