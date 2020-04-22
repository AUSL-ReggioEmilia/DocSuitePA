
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import LocationTypeEnum = require('App/Models/Commons/LocationTypeEnum');
import EnumHelper = require('App/Helpers/EnumHelper');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import ContainerViewModelMapper = require('App/Mappers/Commons/ContainerViewModelMapper');

class ContainerService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getDossierInsertAuthorizedContainers(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/ContainerService.GetDossierInsertAuthorizedContainers()");
        this.getRequest(url, undefined, (response: any) => {
            if (callback) callback(response.value);
        }, error);
    }

    getAnyDossierAuthorizedContainers(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/ContainerService.GetAnyDossierAuthorizedContainers()");
        this.getRequest(url, undefined, (response: any) => {
            if (callback) callback(response.value);
        }, error);
    }

    getContainers(location?: LocationTypeEnum, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("?$orderby=Name&$filter=isActive eq 1");
        if (location || location == 0) {
            let loc: string = new EnumHelper().getLocationTypeDescription(location);
            url = url.concat(" and not(", loc, " eq null)");
        }
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                let containerMapper: ContainerModelMapper = new ContainerModelMapper();
                callback(containerMapper.MapCollection(response.value));
            }
        }, error);
    }

    getContainersByName(name: string,location?: LocationTypeEnum, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("?$orderby=Name&$filter=contains(Name,'".concat(name, "') and isActive eq 1"));

        if (location || location == 0) {
            let loc: string = new EnumHelper().getLocationTypeDescription(location);
            url = url.concat(" and not(", loc, " eq null)");
        }

        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                let containerMapper: ContainerModelMapper = new ContainerModelMapper();
                callback(containerMapper.MapCollection(response.value));
            }
        }, error);
    }

    getContainersByNameFascicleRights(name: string, location?: LocationTypeEnum, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("?$orderby=Name&$filter=contains(Name,'".concat(name, "') and isActive eq 1 and ContainerGroups/any(cg:cg/FascicleRights ne null and cg/FascicleRights ne '00000000000000000000')"));

        if (location || location == 0) {
            let loc: string = new EnumHelper().getLocationTypeDescription(location);
            url = url.concat(" and not(", loc, " eq null)");
        }

        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                let containerMapper: ContainerModelMapper = new ContainerModelMapper();
                callback(containerMapper.MapCollection(response.value));
            }
        }, error);
    }

    getContainersByIdCategory(idCategory: string, location?: LocationTypeEnum, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
            //CategoryFascicleRights/any(cfr:cfr/CategoryFascicle/Category/EntityShortId eq -31428)
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("?$orderby=Name&$filter=CategoryFascicleRights/any(cfr:cfr/CategoryFascicle/Category/EntityShortId eq ".concat(idCategory, ")"));

        if (location || location == 0) {
            let loc: string = new EnumHelper().getLocationTypeDescription(location);
            url = url.concat(" and not(", loc, " eq null)");
        }

        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                let containerMapper: ContainerModelMapper = new ContainerModelMapper();
                callback(containerMapper.MapCollection(response.value));
            }
        }, error);
    }

    getAllContainers(name: string, topElement: string, skipElement: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=contains(Name,'".concat(name, "') and isActive eq 1 &$orderby=Name&$count=true&$top=", topElement, "&$skip=", skipElement.toString());

        this.getRequest(url, qs, (response: any) => {
            if (callback) {
                let responseModel: ODATAResponseModel<ContainerModel> = new ODATAResponseModel<ContainerModel>(response);

                let mapper = new ContainerModelMapper();
                responseModel.value = mapper.MapCollection(response.value);;

                callback(responseModel);
            }
        }, error);
    }

    getContainersByEnvironment(name: string, topElement: string, skipElement: number, location?: LocationTypeEnum, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=";
        if (location || location == 0) {
            let loc: string = new EnumHelper().getLocationTypeDescription(location);
            qs = qs.concat("not(", loc, " eq null)");
        } else {
            qs = qs.concat("(ProtLocation ne null or ReslLocation ne null or UDSLocation ne null)");
        }
        qs = qs.concat(" and contains(Name,'".concat(name, "') and isActive eq 1 &$orderby=Name&$count=true&$top=", topElement, "&$skip=", skipElement.toString()));

        this.getRequest(url, qs, (response: any) => {
            if (callback) {
                let responseModel: ODATAResponseModel<ContainerModel> = new ODATAResponseModel<ContainerModel>(response);

                let mapper = new ContainerModelMapper();
                responseModel.value = mapper.MapCollection(response.value);;

                callback(responseModel);
            }
        }, error);
    }

    getInsertAuthorizedContainers(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/ContainerService.GetInsertAuthorizedContainers()");
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let responseModel: ContainerModel[];
                let mapper = new ContainerModelMapper();
                responseModel = mapper.MapCollection(response.value);
                callback(responseModel);
            }
        }, error);
    }

    getFascicleInsertAuthorizedContainers(idCategory?: number, fascicleType?: FascicleType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let fascicleTypeParam: string = null;
        if (fascicleType) {
            fascicleTypeParam = `'${FascicleType[fascicleType]}'`;
        }

        let url: string = this._configuration.ODATAUrl.concat(`/ContainerService.GetFascicleInsertAuthorizedContainers(idCategory=${idCategory},fascicleType=${fascicleTypeParam})`);
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let mapper = new ContainerViewModelMapper();
                let responseModel: ContainerModel[] = mapper.MapCollection(response.value);
                callback(responseModel);
            }
        }, error);
    }
}

export = ContainerService;