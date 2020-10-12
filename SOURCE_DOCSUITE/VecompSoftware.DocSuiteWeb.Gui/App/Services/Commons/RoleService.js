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
define(["require", "exports", "App/Services/BaseService", "App/Models/ODATAResponseModel", "App/Mappers/Commons/RoleModelMapper"], function (require, exports, BaseService, ODATAResponseModel, RoleModelMapper) {
    var RoleService = /** @class */ (function (_super) {
        __extends(RoleService, _super);
        /**
        * Costruttore
        */
        function RoleService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /*
        *
        */
        RoleService.prototype.getDossierFolderRole = function (dossierFolderId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=DossierFolderRoles".concat("($filter=DossierFolder/UniqueId eq ", dossierFolderId, ")&$filter=DossierFolderRoles/any(d:d/DossierFolder/UniqueId eq ", dossierFolderId, ")");
            this.getRequest(url, data, function (response) {
                if (callback) {
                    if (response) {
                        callback(response.value);
                    }
                }
            }, error);
        };
        RoleService.prototype.getTenantRoles = function (tenantId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Father&$filter=Tenants/any(t: t/UniqueId eq " + tenantId + ")&$orderby=Name";
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    var modelMapper_1 = new RoleModelMapper();
                    var roles_1 = [];
                    $.each(response.value, function (i, value) {
                        roles_1.push(modelMapper_1.Map(value));
                    });
                    callback(roles_1);
                }
                ;
            }, error);
        };
        RoleService.prototype.getRoles = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper_2 = new RoleModelMapper();
                    var roles_2 = [];
                    $.each(response.value, function (i, value) {
                        roles_2.push(modelMapper_2.Map(value));
                    });
                    callback(roles_2);
                }
                ;
            }, error);
        };
        RoleService.prototype.findRoles = function (finderModel, callback, error) {
            var odataUrl = this._configuration.ODATAUrl + "/RoleService.FindRoles(finder=@p0)?@p0=" + JSON.stringify(finderModel) + "&$orderby=Name";
            this.getRequest(odataUrl, null, function (response) {
                if (callback && response) {
                    var modelMapper_3 = new RoleModelMapper();
                    var roles_3 = [];
                    $.each(response.value, function (i, value) {
                        roles_3.push(modelMapper_3.Map(value));
                    });
                    callback(roles_3);
                }
            }, error);
        };
        RoleService.prototype.findRole = function (roleId, callback, error) {
            var odataUrl = this._configuration.ODATAUrl;
            var data = "$expand=Father&$filter=EntityShortId eq " + roleId;
            this.getRequest(odataUrl, data, function (response) {
                if (callback && response) {
                    var modelMapper = new RoleModelMapper();
                    var roleModel = modelMapper.Map(response.value[0]);
                    callback(roleModel);
                }
            }, error);
        };
        RoleService.prototype.getAllRoles = function (name, topElement, skipElement, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=contains(Name,'" + name + "')&$count=true&$top=" + topElement + "&$skip=" + skipElement.toString();
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    var mapper = new RoleModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);
                    ;
                    callback(responseModel);
                }
            }, error);
        };
        RoleService.prototype.getDocumentUnitRoles = function (idDocumentUnitRole, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=UniqueId eq " + idDocumentUnitRole;
            this.getRequest(url, qs, function (response) {
                if (callback && response) {
                    var instance = new RoleModelMapper();
                    callback(instance.Map(response.value[0]));
                }
                ;
            }, error);
        };
        RoleService.prototype.hasCategoryFascicleRole = function (idCategory, callback, error) {
            var url = this._configuration.ODATAUrl + "/RoleService.HasCategoryFascicleRole(idCategory=" + idCategory + ")";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
        };
        return RoleService;
    }(BaseService));
    return RoleService;
});
//# sourceMappingURL=RoleService.js.map