define(["require", "exports"], function (require, exports) {
    var ServiceConfigurationHelper = /** @class */ (function () {
        function ServiceConfigurationHelper() {
        }
        ServiceConfigurationHelper.getService = function (serviceConfigurations, name) {
            return $.grep(serviceConfigurations, function (x) { return x.Name == name; })[0];
        };
        return ServiceConfigurationHelper;
    }());
    return ServiceConfigurationHelper;
});
//# sourceMappingURL=ServiceConfigurationHelper.js.map