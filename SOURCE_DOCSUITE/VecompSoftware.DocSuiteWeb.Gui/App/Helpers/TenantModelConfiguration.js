define(["require", "exports"], function (require, exports) {
    var TenantModelconfiguration = /** @class */ (function () {
        function TenantModelconfiguration(jsonModel) {
            var reportBuilderModel = JSON.parse(jsonModel);
            this.serviceConfiguration = reportBuilderModel;
        }
        return TenantModelconfiguration;
    }());
    return TenantModelconfiguration;
});
//# sourceMappingURL=TenantModelConfiguration.js.map