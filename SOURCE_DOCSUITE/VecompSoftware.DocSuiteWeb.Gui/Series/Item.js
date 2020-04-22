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
    var Item = /** @class */ (function (_super) {
        __extends(Item, _super);
        function Item(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME)) || this;
            $(document).ready(function () {
            });
            return _this;
        }
        Item.prototype.initialize = function () {
            this._btnNuovo = document.getElementById(this.btnNuovoMonitoraggioId);
        };
        Item.prototype.showWindow = function () {
            this._windowNuovo = $find(this.uscAmmTraspMonitorLogId);
            this._windowNuovo.show();
        };
        return Item;
    }(TransparentAdministrationMonitorLogBase));
    return Item;
});
//# sourceMappingURL=Item.js.map