/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
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
define(["require", "exports", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "../app/core/extensions/string"], function (require, exports, FascicleBase, ServiceConfigurationHelper) {
    var FascDocumentsInsert = /** @class */ (function (_super) {
        __extends(FascDocumentsInsert, _super);
        /**
         * Costruttore
        */
        function FascDocumentsInsert(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        *------------------------- Events -----------------------------
        */
        /**
        *------------------------- Methods -----------------------------
        */
        FascDocumentsInsert.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._notificationInfo = $find(this.radNotificationInfoId);
            this._manager = $find(this.radWindowManagerId);
        };
        return FascDocumentsInsert;
    }(FascicleBase));
    return FascDocumentsInsert;
});
//# sourceMappingURL=FascDocumentsInsert.js.map