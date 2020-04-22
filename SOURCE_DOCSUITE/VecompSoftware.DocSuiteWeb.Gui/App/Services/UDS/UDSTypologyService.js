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
define(["require", "exports", "App/Mappers/UDS/UDSTypologyModelMapper", "App/Services/BaseService"], function (require, exports, UDSTypologyModelMapper, BaseService) {
    var UDSTypologyService = /** @class */ (function (_super) {
        __extends(UDSTypologyService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function UDSTypologyService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        UDSTypologyService.prototype.getUDSTypologyById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq ".concat(uniqueId);
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new UDSTypologyModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
        };
        /**
         * Recupera una UDSRepository per Nome
         * @param name
         * @param callback
         * @param error
         */
        UDSTypologyService.prototype.getUDSTypologyByName = function (name, status, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=contains(Name,'".concat(name.toString(), "')");
            if (status) {
                data = data.concat("and Status eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSTypologyStatus'", status.toString(), "'");
            }
            data = data.concat("&$orderby=Name asc");
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new UDSTypologyModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        UDSTypologyService.prototype.insertUDSTypology = function (typology, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(typology), callback, error);
        };
        /**
        * Aggiorno una UDSTypology
        */
        UDSTypologyService.prototype.updateUDSTypology = function (typology, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(typology), callback, error);
        };
        return UDSTypologyService;
    }(BaseService));
    return UDSTypologyService;
});
//# sourceMappingURL=UDSTypologyService.js.map