import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import UpdateActionType = require("App/Models/UpdateActionType");
import RoleModel = require('App/Models/Commons/RoleModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import IRoleService = require('App/Services/Commons/IRoleService');
import RoleModelMapper = require('App/Mappers/Commons/RoleModelMapper');
import RoleSearchFilterDTO = require('App/DTOs/RoleSearchFilterDTO');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');


class RoleService extends BaseService implements IRoleService {
    private _configuration: ServiceConfiguration;

    /**
    * Costruttore 
    */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    /*
    *
    */
    getDossierFolderRole(dossierFolderId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$expand=DossierFolderRoles".concat("($filter=DossierFolder/UniqueId eq ", dossierFolderId, ")&$filter=DossierFolderRoles/any(d:d/DossierFolder/UniqueId eq ", dossierFolderId, ")");
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    if (response) {
                        callback(response.value)
                    }
                }
            }, error);
    }

    getRoles(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new RoleModelMapper();
                let roles: RoleModel[] = [];
                $.each(response.value, function (i, value) {
                    roles.push(modelMapper.Map(value));
                });
                callback(roles);
            };
        }, error);
    }

    findRoles(finderModel: RoleSearchFilterDTO, callback?: (data: RoleModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataUrl: string = `${this._configuration.ODATAUrl}/RoleService.FindRoles(finder=@p0)?@p0=${JSON.stringify(finderModel)}&$orderby=Name`;

        this.getRequest(odataUrl, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new RoleModelMapper();
                let roles: RoleModel[] = [];
                $.each(response.value, function (i, value) {
                    roles.push(modelMapper.Map(value));
                });
                callback(roles);
            }
        }, error);
    }

    countRoles(idTenantAOO: string, callback?: (data: number) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataUrl: string = `${this._configuration.ODATAUrl}/$count?$filter=TenantAOO/UniqueId eq ${idTenantAOO} and IsActive eq true`;

        this.getRequest(odataUrl, null, (response: any) => {
            if (callback && response) {
                callback(response.value);
            }
        }, error);
    }

    findRole(roleId: number, callback?: (data: RoleModel) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataUrl: string = this._configuration.ODATAUrl;
        let data: string = `$expand=Father&$filter=EntityShortId eq ${roleId}`;

        this.getRequest(odataUrl, data, (response: any) => {
            if (callback && response) {
                let modelMapper = new RoleModelMapper();
                let roleModel: RoleModel = modelMapper.Map(response.value[0]);
                callback(roleModel);
            }
        }, error);
    }

    getAllRoles(name: string, topElement: string,
        skipElement: number,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=contains(Name,'${name}')&$count=true&$top=${topElement}&$skip=${skipElement.toString()}`;

        this.getRequest(url,
            qs,
            (response: any) => {
                if (callback) {
                    let responseModel: ODATAResponseModel<RoleModel> = new ODATAResponseModel<RoleModel>(response);

                    let mapper = new RoleModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);;

                    callback(responseModel);
                }
            },
            error);
    }

    getDocumentUnitRoles(idDocumentUnitRole: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=UniqueId eq ${idDocumentUnitRole}`;
        this.getRequest(url, qs, (response: any) => {
            if (callback && response) {
                let instance = new RoleModelMapper();
                callback(instance.Map(response.value[0]));
            };
        }, error);
    }

    hasCategoryFascicleRole(idCategory: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}/RoleService.HasCategoryFascicleRole(idCategory=${idCategory})`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                callback(response.value);
            }
        }, error);
    }

} export = RoleService;