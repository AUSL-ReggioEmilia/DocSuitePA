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
    var DomainUserService = /** @class */ (function (_super) {
        __extends(DomainUserService, _super);
        function DomainUserService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        DomainUserService.prototype.getUser = function (account, callback, error) {
            var username = account.substring(account.lastIndexOf("\\") + 1);
            var domain = account.substring(0, account.indexOf("\\"));
            var url = this._configuration.ODATAUrl.concat("/DomainUserService.GetUser(username='", username, "',domain='", domain, "')");
            var data = "";
            this.getRequest(url, data, function (response) {
                if (callback)
                    callback(response);
            }, error);
        };
        DomainUserService.prototype.userFinder = function (account, callback, error) {
            var username = account.substring(account.lastIndexOf("\\") + 1);
            var url = this._configuration.ODATAUrl.concat("/DomainUserService.UsersFinder(text='", username, "')");
            var data = "";
            this.getRequest(url, data, function (response) {
                if (callback)
                    callback(response.value);
            }, error);
        };
        return DomainUserService;
    }(BaseService));
    return DomainUserService;
});
//# sourceMappingURL=DomainUserService.js.map