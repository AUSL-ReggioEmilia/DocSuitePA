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
    var ContactPlaceNameService = /** @class */ (function (_super) {
        __extends(ContactPlaceNameService, _super);
        function ContactPlaceNameService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        ContactPlaceNameService.prototype.getAll = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        return ContactPlaceNameService;
    }(BaseService));
    return ContactPlaceNameService;
});
//# sourceMappingURL=ContactPlaceNameService.js.map