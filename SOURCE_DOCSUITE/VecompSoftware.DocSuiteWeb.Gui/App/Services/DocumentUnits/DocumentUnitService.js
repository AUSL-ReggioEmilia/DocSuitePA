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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/DocumentUnits/DocumentUnitModelMapper", "App/Models/Environment"], function (require, exports, BaseService, DocumentUnitModelMapper, Environment) {
    var DocumentUnitService = /** @class */ (function (_super) {
        __extends(DocumentUnitService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function DocumentUnitService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /**
         * Recupera le Document Units per anno/numero e utente specifico
         * @param year
         * @param number
         * @param username
         * @param domain
         * @param isSecurityEnabled
         * @param callback
         * @param error
         */
        DocumentUnitService.prototype.findDocumentUnits = function (finderModel, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DocumentUnitService.AuthorizedDocumentUnits(finder=@p0)?@p0=" + JSON.stringify(finderModel));
            this.getRequest(url, null, function (response) {
                var instances = new Array();
                var mapper = new DocumentUnitModelMapper();
                instances = mapper.MapCollection(response.value);
                if (callback)
                    callback(instances);
            }, error);
        };
        /**
       * Recupera il count totale delle document units
       */
        DocumentUnitService.prototype.countDocumentUnits = function (finderModel, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/DocumentUnitService.CountAuthorizedDocumentUnits(finder=@p0)?@p0=" + JSON.stringify(finderModel));
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
                ;
            }, error);
        };
        /**
         * Recupera le Document Units associate al Fascicolo
         * TODO: da implementare in SignalR
         * @param model
         * @param qs
         * @param callback
         * @param error
         */
        DocumentUnitService.prototype.getFascicleDocumentUnits = function (model, qs, idTenantAOO, idFascicleFolder, callback, error) {
            if (!idFascicleFolder) {
                idFascicleFolder = null;
            }
            var url = this._configuration.ODATAUrl.concat("/DocumentUnitService.FascicleDocumentUnits(idFascicle=@p1,idFascicleFolder=@p2,idTenantAOO=@p3)?@p1=" + model.UniqueId + "&@p2=" + idFascicleFolder + "&@p3=" + idTenantAOO);
            this.getRequest(url, qs, function (response) {
                if (callback)
                    callback(response.value);
            }, error);
        };
        DocumentUnitService.prototype.getDocumentUnitEnvironment = function (documentUnit) {
            var env = (documentUnit.Environment < 100 ? documentUnit.Environment : Environment.UDS);
            return env;
        };
        DocumentUnitService.prototype.getDocumentUnitById = function (idDocumentUnit, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq " + idDocumentUnit + "&$expand=Category,Container,UDSRepository,DocumentUnitRoles,DocumentUnitChains";
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new DocumentUnitModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
        };
        DocumentUnitService.prototype.getDocumentUnitsFullText = function (fullTextSearch, idTenant, callback, error) {
            fullTextSearch = fullTextSearch.replace("'", "''");
            var url = this._configuration.ODATAUrl.concat("/DocumentUnitService.FullTextSearchDocumentUnits(filter='" + fullTextSearch + "',idTenant=" + idTenant + ")");
            this.getRequest(url, null, function (response) {
                if (callback)
                    callback(response.value);
            }, error);
        };
        return DocumentUnitService;
    }(BaseService));
    return DocumentUnitService;
});
//# sourceMappingURL=DocumentUnitService.js.map