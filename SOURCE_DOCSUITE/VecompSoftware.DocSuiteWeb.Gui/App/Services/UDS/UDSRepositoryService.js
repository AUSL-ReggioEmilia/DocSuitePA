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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/UDS/UDSRepositoryModelMapper"], function (require, exports, BaseService, UDSRepositoryModelMapper) {
    var UDSRepositoryService = /** @class */ (function (_super) {
        __extends(UDSRepositoryService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function UDSRepositoryService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /**
         * Recupera una UDSRepository per Nome
         * @param name
         * @param callback
         * @param error
         */
        UDSRepositoryService.prototype.getUDSRepositoryByName = function (name, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=Name eq '".concat(name.toString(), "' and Status eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSRepositoryStatus'2'&$orderby=Version desc&$top=1");
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
        };
        /**
     * Recupera una UDSRepository per Nome
     * @param name
     * @param callback
     * @param error
     */
        UDSRepositoryService.prototype.getUDSRepositoryByDSWEnvironment = function (DSWEnvironment, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=DSWEnvironment eq ".concat(DSWEnvironment.toString(), "&$orderby=Version desc&$top=1");
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
        };
        /**
         * Recupera una UDSRepository per Environment
         * @param environment
         * @param callback
         * @param error
         */
        UDSRepositoryService.prototype.getUDSRepositoryByEnvironment = function (environment, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=Environment eq '".concat(environment.toString(), "' and Status eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSRepositoryStatus'2'&$orderby=Version desc&$top=1");
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
        };
        UDSRepositoryService.prototype.getUDSRepositoryByUDSTypology = function (typologyId, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/?$filter=UDSTypologies/any(s:s/UniqueId eq ", typologyId, ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var mapper = new UDSRepositoryModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        UDSRepositoryService.prototype.getUDSRepositoryByUDSTypologyName = function (typologyName, tentantName, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/?$filter=UDSTypologies/any(s:s/Name eq '", typologyName, "')");
            if (tentantName != "") {
                url = url.concat(" and startsWith(Name", ",'", tentantName, " - ')");
            }
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var mapper = new UDSRepositoryModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        UDSRepositoryService.prototype.getUDSRepositories = function (tentantName, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("?$filter=Status eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSRepositoryStatus'2' and ExpiredDate eq null");
            if (tentantName != "") {
                url = url.concat(" and startsWith(Name", ",'", tentantName, " - ')");
            }
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var mapper = new UDSRepositoryModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        UDSRepositoryService.prototype.getTenantUDSRepositories = function (tenantName, udsName, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/UDSRepositoryService.GetTenantUDSRepositories(tenantName='", tenantName, "',udsName='", udsName, "')");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var mapper = new UDSRepositoryModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        UDSRepositoryService.prototype.getUDSRepositoryByID = function (udsID, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/?$filter=UniqueId eq ", udsID);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var mapper = new UDSRepositoryModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        UDSRepositoryService.prototype.getAvailableCQRSPublishedUDSRepositories = function (typologyId, name, alias, idContainer, callback, error) {
            var url = this._configuration.ODATAUrl;
            var container = null;
            var nameUDSRepository = "";
            var aliasUDSRepository = "";
            if (idContainer) {
                container = idContainer.toString();
            }
            if (name) {
                nameUDSRepository = name.toString();
            }
            if (alias) {
                aliasUDSRepository = alias.toString();
            }
            url = url.concat("/UDSRepositoryService.GetAvailableCQRSPublishedUDSRepositories(idUDSTypology=", typologyId, ",name='", nameUDSRepository, "',alias='", aliasUDSRepository, "',idContainer=", container, ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var mapper = new UDSRepositoryModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        UDSRepositoryService.prototype.getInsertableRepositoriesByTypology = function (username, domain, typologyId, pecAnnexedEnabled, callback, error) {
            var url = this._configuration.ODATAUrl;
            if (!typologyId) {
                typologyId = null;
            }
            url = url.concat("/UDSRepositoryService.GetInsertableRepositoriesByTypology(username='" + username + "',domain='" + domain + "',idUDSTypology=" + typologyId + ",pecAnnexedEnabled=" + pecAnnexedEnabled + ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var mapper = new UDSRepositoryModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        return UDSRepositoryService;
    }(BaseService));
    return UDSRepositoryService;
});
//# sourceMappingURL=UDSRepositoryService.js.map