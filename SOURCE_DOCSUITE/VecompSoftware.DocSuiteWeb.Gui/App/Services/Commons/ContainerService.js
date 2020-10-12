var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Mappers/Commons/ContainerModelMapper", "App/Services/BaseService", "App/Helpers/EnumHelper", "App/Models/ODATAResponseModel", "App/Models/Fascicles/FascicleType", "App/Mappers/Commons/ContainerViewModelMapper"], function (require, exports, ContainerModelMapper, BaseService, EnumHelper, ODATAResponseModel, FascicleType, ContainerViewModelMapper) {
    var ContainerService = /** @class */ (function (_super) {
        __extends(ContainerService, _super);
        /**
         * Costruttore
         */
        function ContainerService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        ContainerService.prototype.getDossierInsertAuthorizedContainers = function (tenantId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/ContainerService.GetDossierInsertAuthorizedContainers(tenantId=" + tenantId + ")");
            this.getRequest(url, undefined, function (response) {
                if (callback)
                    callback(response.value);
            }, error);
        };
        ContainerService.prototype.getAnyDossierAuthorizedContainers = function (tenantId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/ContainerService.GetAnyDossierAuthorizedContainers(tenantId=" + tenantId + ")");
            this.getRequest(url, undefined, function (response) {
                if (callback)
                    callback(response.value);
            }, error);
        };
        ContainerService.prototype.getContainers = function (location, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("?$orderby=Name&$filter=isActive eq 1");
            if (location || location == 0) {
                var loc = new EnumHelper().getLocationTypeDescription(location);
                url = url.concat(" and not(", loc, " eq null)");
            }
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var containerMapper = new ContainerModelMapper();
                    callback(containerMapper.MapCollection(response.value));
                }
            }, error);
        };
        ContainerService.prototype.getContainersByName = function (name, location, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("?$orderby=Name&$filter=contains(Name,'".concat(name, "') and isActive eq 1"));
            if (location || location == 0) {
                var loc = new EnumHelper().getLocationTypeDescription(location);
                url = url.concat(" and not(", loc, " eq null)");
            }
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var containerMapper = new ContainerModelMapper();
                    callback(containerMapper.MapCollection(response.value));
                }
            }, error);
        };
        ContainerService.prototype.getContainersByNameFascicleRights = function (name, location, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("?$orderby=Name&$filter=contains(Name,'".concat(name, "') and isActive eq 1 and ContainerGroups/any(cg:cg/FascicleRights ne null and cg/FascicleRights ne '00000000000000000000')"));
            if (location || location == 0) {
                var loc = new EnumHelper().getLocationTypeDescription(location);
                url = url.concat(" and not(", loc, " eq null)");
            }
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var containerMapper = new ContainerModelMapper();
                    callback(containerMapper.MapCollection(response.value));
                }
            }, error);
        };
        ContainerService.prototype.getContainersByIdCategory = function (idCategory, location, callback, error) {
            //CategoryFascicleRights/any(cfr:cfr/CategoryFascicle/Category/EntityShortId eq -31428)
            var url = this._configuration.ODATAUrl;
            url = url.concat("?$orderby=Name&$filter=CategoryFascicleRights/any(cfr:cfr/CategoryFascicle/Category/EntityShortId eq ".concat(idCategory, ")"));
            if (location || location == 0) {
                var loc = new EnumHelper().getLocationTypeDescription(location);
                url = url.concat(" and not(", loc, " eq null)");
            }
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var containerMapper = new ContainerModelMapper();
                    callback(containerMapper.MapCollection(response.value));
                }
            }, error);
        };
        ContainerService.prototype.getAllContainers = function (name, topElement, skipElement, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=contains(Name,'".concat(name, "') and isActive eq 1 &$orderby=Name&$count=true&$top=", topElement, "&$skip=", skipElement.toString());
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    var mapper = new ContainerModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);
                    ;
                    callback(responseModel);
                }
            }, error);
        };
        ContainerService.prototype.getContainersByEnvironment = function (name, topElement, skipElement, location, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=";
            if (location || location == 0) {
                var loc = new EnumHelper().getLocationTypeDescription(location);
                qs = qs.concat("not(", loc, " eq null)");
            }
            else {
                qs = qs.concat("(ProtLocation ne null or ReslLocation ne null or UDSLocation ne null)");
            }
            qs = qs.concat(" and contains(Name,'".concat(name, "') and isActive eq 1 &$orderby=Name&$count=true&$top=", topElement, "&$skip=", skipElement.toString()));
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    var mapper = new ContainerModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);
                    ;
                    callback(responseModel);
                }
            }, error);
        };
        ContainerService.prototype.getInsertAuthorizedContainers = function (callback, error) {
            var url = this._configuration.ODATAUrl.concat("/ContainerService.GetInsertAuthorizedContainers()");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var responseModel = void 0;
                    var mapper = new ContainerModelMapper();
                    responseModel = mapper.MapCollection(response.value);
                    callback(responseModel);
                }
            }, error);
        };
        ContainerService.prototype.getFascicleInsertAuthorizedContainers = function (idCategory, fascicleType, callback, error) {
            var fascicleTypeParam = null;
            if (fascicleType) {
                fascicleTypeParam = "'" + FascicleType[fascicleType] + "'";
            }
            var url = this._configuration.ODATAUrl.concat("/ContainerService.GetFascicleInsertAuthorizedContainers(idCategory=" + idCategory + ",fascicleType=" + fascicleTypeParam + ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var mapper = new ContainerViewModelMapper();
                    var responseModel = mapper.MapCollection(response.value);
                    callback(responseModel);
                }
            }, error);
        };
        return ContainerService;
    }(BaseService));
    return ContainerService;
});
//# sourceMappingURL=ContainerService.js.map