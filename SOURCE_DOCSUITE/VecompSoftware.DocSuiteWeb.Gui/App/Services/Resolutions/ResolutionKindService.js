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
    var ResolutionKindService = /** @class */ (function (_super) {
        __extends(ResolutionKindService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function ResolutionKindService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        ResolutionKindService.prototype.findActiveTypologies = function (callback, error) {
            var qs = "$filter=IsActive eq true";
            this.findTypologies(qs, callback, error);
        };
        ResolutionKindService.prototype.findDisabledTypologies = function (callback, error) {
            var qs = "$filter=IsActive eq false";
            this.findTypologies(qs, callback, error);
        };
        ResolutionKindService.prototype.findAllTypologies = function (callback, error) {
            var qs = "$filter=IsActive eq true or IsActive eq false";
            this.findTypologies(qs, callback, error);
        };
        ResolutionKindService.prototype.findTypologies = function (qs, callback, error) {
            qs = qs.concat("&$orderby=Name");
            this.getRequest(this._configuration.ODATAUrl, qs, function (response) {
                if (callback && response) {
                    var resolutionKinds = response.value;
                    callback(resolutionKinds);
                }
            }, error);
        };
        ResolutionKindService.prototype.getById = function (idResolutionKind, callback, error) {
            var qs = "$filter=UniqueId eq ".concat(idResolutionKind);
            this.getRequest(this._configuration.ODATAUrl, qs, function (response) {
                if (callback && response) {
                    var resolutionKind = response.value;
                    callback(resolutionKind[0]);
                }
            }, error);
        };
        ResolutionKindService.prototype.insertResolutionKindModel = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        ResolutionKindService.prototype.updateResolutionKindModel = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        ResolutionKindService.prototype.deleteResolutionKindModel = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        return ResolutionKindService;
    }(BaseService));
    return ResolutionKindService;
});
//# sourceMappingURL=ResolutionKindService.js.map