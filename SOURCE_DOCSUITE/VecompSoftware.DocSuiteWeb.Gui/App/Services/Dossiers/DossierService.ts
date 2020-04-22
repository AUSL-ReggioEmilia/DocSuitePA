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
import DossierSummaryViewModel = require('../../ViewModels/Dossiers/DossierSummaryViewModel');

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

    getDossiers(skip: number, top: number, searchFilter: DossierSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {

        let url: string = this._configuration.ODATAUrl.
            concat("/DossierService.GetAuthorizedDossiers(skip=", skip.toString(), ",top=", top.toString(),
                ",year=", !!searchFilter.year ? searchFilter.year.toString() : null,
                ",number=", !!searchFilter.number ? searchFilter.number.toString() : null,
                ",subject=\'", searchFilter.subject,
                "\',note=\'", searchFilter.note,
                "\',idContainer=", !!searchFilter.idContainer ? searchFilter.idContainer.toString() : null,
                ",startDateFrom=\'", searchFilter.startDateFrom,
                "\',startDateTo=\'", searchFilter.startDateTo,
                "\',endDateFrom=\'", searchFilter.endDateFrom,
                "\',endDateTo=\'", searchFilter.endDateTo,
                "\',idMetadataRepository=", searchFilter.idMetadataRepository ? searchFilter.idMetadataRepository.toString() : null,
                "\,metadataValue=\'", searchFilter.metadataValue,
                "\')");

        this.getRequest(url, null, (response: any) => {
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

    countDossiers(searchFilter: DossierSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.
            concat("/DossierService.CountAuthorizedDossiers(year=", !!searchFilter.year ? searchFilter.year.toString() : null, ",number=", !!searchFilter.number ? searchFilter.number.toString() : null, ",subject=\'",
                searchFilter.subject, "\',idContainer=", !!searchFilter.idContainer ? searchFilter.idContainer.toString() : null, ",idMetadataRepository=", !!searchFilter.idMetadataRepository ? searchFilter.idMetadataRepository.toString() : null, ",metadataValue='", searchFilter.metadataValue, "')");
        this.getRequest(url, null, (response: any) => {
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


    countDossiersById(uniqueId: string, onlyProcess: boolean, exludeProcess: boolean, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DossierFolders/any(d: d/Fascicle/Uniqueid eq ${uniqueId})`;
        if (onlyProcess && onlyProcess === true) {
            data = `${data} and Processes/any()`
        }
        if (exludeProcess && exludeProcess === true) {
            data = `${data} and not Processes/any()`
        }
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

}

export = DossierService; 