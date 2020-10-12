define(["require", "exports", "App/Services/UDS/UDSConnectionService", "App/Services/UDS/UDSRepositoryService", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, UDSConnectionService, UDSRepositoryService, ServiceConfigurationHelper) {
    var UDSViewBase = /** @class */ (function () {
        function UDSViewBase(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
        }
        UDSViewBase.prototype.initialize = function () {
            var udsConnectionConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, UDSViewBase.UDS_DOCUMENT_UNIT_NAME);
            var udsRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, UDSViewBase.UDS_REPOSITORY_NAME);
            this.udsConnectionService = new UDSConnectionService(udsConnectionConfiguration);
            this.udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
        };
        UDSViewBase.UDS_DOCUMENT_UNIT_NAME = "UDSDocumentUnit";
        UDSViewBase.UDS_REPOSITORY_NAME = "UDSRepository";
        return UDSViewBase;
    }());
    return UDSViewBase;
});
//# sourceMappingURL=UDSViewBase.js.map