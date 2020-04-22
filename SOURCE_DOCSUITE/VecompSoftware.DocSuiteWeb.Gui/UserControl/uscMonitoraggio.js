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
define(["require", "exports", "Monitors/TransparentAdministrationMonitorLogBase", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, TransparentAdministrationMonitorLogBase, ServiceConfigurationHelper) {
    var uscMonitoraggio = /** @class */ (function (_super) {
        __extends(uscMonitoraggio, _super);
        function uscMonitoraggio(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME)) || this;
            $(document).ready(function () {
            });
            return _this;
        }
        uscMonitoraggio.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
        };
        return uscMonitoraggio;
    }(TransparentAdministrationMonitorLogBase));
    return uscMonitoraggio;
});
//# sourceMappingURL=uscMonitoraggio.js.map