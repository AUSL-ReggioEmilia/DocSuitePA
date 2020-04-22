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
define(["require", "exports", "App/Services/BaseService", "../../Mappers/UserLogs/UserLogsMapper"], function (require, exports, BaseService, UserLogsMapper) {
    var UserLogsService = /** @class */ (function (_super) {
        __extends(UserLogsService, _super);
        function UserLogsService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        UserLogsService.prototype.getUserDetailBySystemUser = function (name, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=SystemUser eq '" + name + "'";
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var modelMapper_1 = new UserLogsMapper();
                    var userLogs_1 = [];
                    $.each(response.value, function (i, value) {
                        userLogs_1.push(modelMapper_1.Map(value));
                    });
                    callback(userLogs_1);
                }
            }, error);
        };
        return UserLogsService;
    }(BaseService));
    return UserLogsService;
});
//# sourceMappingURL=UserLogsService.js.map