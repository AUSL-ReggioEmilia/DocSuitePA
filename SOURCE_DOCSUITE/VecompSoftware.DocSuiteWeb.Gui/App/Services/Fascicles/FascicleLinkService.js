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
define(["require", "exports", "App/Services/BaseService"], function (require, exports, BaseService) {
    var FascicleLinkService = /** @class */ (function (_super) {
        __extends(FascicleLinkService, _super);
        /**
         * Costruttore
         */
        function FascicleLinkService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        FascicleLinkService.prototype.insertFascicleLink = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        FascicleLinkService.prototype.deleteFascicleLink = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        FascicleLinkService.prototype.getLinkedFasciclesById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Fascicle,FascicleLinked&$filter=Fascicle/UniqueId eq " + uniqueId;
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        FascicleLinkService.prototype.countLinkedFascicleById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=Fascicle/UniqueId eq " + uniqueId;
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        return FascicleLinkService;
    }(BaseService));
    return FascicleLinkService;
});
//# sourceMappingURL=FascicleLinkService.js.map