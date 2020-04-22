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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Resolutions/ResolutionModelMapper"], function (require, exports, BaseService, ResolutionModelMapper) {
    var ResolutionService = /** @class */ (function (_super) {
        __extends(ResolutionService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function ResolutionService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /**
         * Recupera una resolution per ID
         * @param idResolution
         * @param callback
         * @param error
         */
        ResolutionService.prototype.getResolutionById = function (idResolution, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=EntityId eq ".concat(idResolution.toString(), "&$expand=Category");
            this.getRequest(url, data, function (response) {
                var mapper = new ResolutionModelMapper();
                callback(mapper.Map(response.value[0]));
            }, error);
        };
        /**
         * Ritorna una resolution per Year/Number e IdType e verifica le autorizzazioni utente
         * @param year
         * @param number
         * @param idType
         * @param isSecurityEnabled
         * @param callback
         * @param error
         */
        ResolutionService.prototype.getAuthorizedResolution = function (year, number, idType, isSecurityEnabled, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=Year eq ".concat(year.toString(), " and IdType eq ", idType.toString(), " and ServiceNumber eq '", number, "' ");
            if (!isNaN(Number(number))) {
                data = data.concat("or Number eq ", number);
            }
            data = data.concat("&$expand=Category");
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new ResolutionModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
        };
        /**
         * Recupera una resolution per UniqueId
         * @param uniqueId
         * @param callback
         * @param error
         */
        ResolutionService.prototype.getResolutionByUniqueId = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq ".concat(uniqueId.toString(), "&$expand=Category,Container");
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new ResolutionModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
        };
        ResolutionService.prototype.getResolutionMessageByUniqueId = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Messages($expand=MessageContacts,MessageEmails)&$filter=UniqueId eq " + uniqueId;
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new ResolutionModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
        };
        return ResolutionService;
    }(BaseService));
    return ResolutionService;
});
//# sourceMappingURL=ResolutionService.js.map