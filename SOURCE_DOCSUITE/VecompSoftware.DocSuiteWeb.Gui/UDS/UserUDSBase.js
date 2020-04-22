define(["require", "exports", "../App/Services/UDS/UDSRepositoryService", "../App/Services/UDS/UDSLogService", "../App/Helpers/ServiceConfigurationHelper"], function (require, exports, UDSRepositoryService, UDSLogService, ServiceConfigurationHelper) {
    var UserUDSBase = /** @class */ (function () {
        function UserUDSBase(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
        }
        UserUDSBase.prototype.initialize = function () {
            var udsRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, UserUDSBase.UDS_REPOSITORY_NAME);
            this.udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
            var udsLogConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, UserUDSBase.UDS_LOG_NAME);
            this.udsLogService = new UDSLogService(udsLogConfiguration);
        };
        UserUDSBase.UDS_REPOSITORY_NAME = "UDSRepository";
        UserUDSBase.UDS_LOG_NAME = "UDSLog";
        return UserUDSBase;
    }());
    return UserUDSBase;
});
//# sourceMappingURL=UserUDSBase.js.map