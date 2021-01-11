import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import MetadataRepositoryViewModelMapper = require('App/Mappers/Commons/MetadataRepositoryViewModelMapper');
import MetadataRepositoryViewModel = require('App/ViewModels/Commons/MetadataRepositoryViewModel');
import MetadataRepositoryModelMapper = require('App/Mappers/Commons/MetadataRepositoryModelMapper');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');

class MetadataRepositoryService extends BaseService {
    _configuration: ServiceConfiguration;
    _mapper: MetadataRepositoryViewModelMapper;
    _modelMapper: MetadataRepositoryModelMapper;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
        this._mapper = new MetadataRepositoryViewModelMapper();
        this._modelMapper = new MetadataRepositoryModelMapper();
    }

    /**
     * Recupero tutti i MetadataRepository
     * @param callback
     * @param error
     */
    findMetadataRepositories(filter: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "";
        qs = "$orderby=Name";
        if (filter && filter.length > 0) {
            qs = qs.concat("&$filter=contains(Name, '", filter, "')");
        }

        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let metadataRepositories: MetadataRepositoryViewModel[] = [];
                    if (response) {
                        metadataRepositories = this._mapper.MapCollection(response.value);
                    }
                    callback(metadataRepositories);
                }
            }, error);
    }


    getAvailableMetadataRepositories(filter: string, repositoryRestrictions: string[], top: string, skip: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$orderby=Name&$count=true&$top=".concat(top, "&$skip=", skip.toString(), "&$filter=Status eq VecompSoftware.DocSuiteWeb.Entity.Commons.MetadataRepositoryStatus'1'");
        if (!!filter) {
            qs = qs.concat(" and contains(Name, '", filter, "')");
        }

        if (repositoryRestrictions && repositoryRestrictions.length > 0) {
            qs = qs.concat(" and (UniqueId eq ", repositoryRestrictions.join(" or UniqueId eq "), ")");
        }

        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let responseModel: ODATAResponseModel<MetadataRepositoryViewModel> = new ODATAResponseModel<MetadataRepositoryViewModel>(response);
                    responseModel.value = this._mapper.MapCollection(response.value);
                    callback(responseModel);
                }
            }, error);
    }

    getById(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=UniqueId eq ".concat(id);
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let result: MetadataRepositoryViewModel = this._mapper.Map(response.value[0]);
                    callback(result);
                }
            }, error);
    }

    getNameById(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=UniqueId eq ".concat(id, "&$select=Name");
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let result: string = (response.value && response.value.length > 0) ? response.value[0].Name : null;
                    callback(result);
                }
            }, error);
    }

    /**
     * Inserisco un nuovo metadataRepository
     * @param model
     * @param callback
     * @param error
     */
    insertMetadataRepository(model: MetadataRepositoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    getFullModelById(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=UniqueId eq ".concat(id);
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let result: MetadataRepositoryModel = this._modelMapper.Map(response.value[0]);
                    callback(result);
                }
            }, error);
    }

    /**
     * Aggiorno un MetadataRepositoryEsistente
     * @param model
     * @param callback
     * @param error
     */
    updateMetadataRepository(model: MetadataRepositoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }
}
export = MetadataRepositoryService;