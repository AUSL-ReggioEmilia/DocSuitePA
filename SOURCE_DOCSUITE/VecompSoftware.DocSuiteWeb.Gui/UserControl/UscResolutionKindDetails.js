/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports"], function (require, exports) {
    var UscResolutionKindDetails = /** @class */ (function () {
        function UscResolutionKindDetails(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
        }
        UscResolutionKindDetails.prototype.labelStatusControl = function () {
            return $("#".concat(this.lblStatusId));
        };
        /**
         *------------------------- Methods -----------------------------
         */
        UscResolutionKindDetails.prototype.initialize = function () {
            this.bindLoaded();
        };
        UscResolutionKindDetails.prototype.bindLoaded = function () {
            $("#".concat(this.pnlInformationsId)).data(this);
        };
        UscResolutionKindDetails.prototype.loadDetails = function (resolutionKind) {
            if (!resolutionKind) {
                return;
            }
            this.labelStatusControl().html((resolutionKind.IsActive) ? "Attivo" : "Disattivo");
        };
        return UscResolutionKindDetails;
    }());
    return UscResolutionKindDetails;
});
//# sourceMappingURL=UscResolutionKindDetails.js.map