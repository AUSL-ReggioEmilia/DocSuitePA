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
define(["require", "exports", "App/Services/BaseService", "App/Models/InsertActionType", "App/Models/FascicolableActionType"], function (require, exports, BaseService, InsertActionType, FascicolableActionType) {
    var FascicolableBaseService = /** @class */ (function (_super) {
        __extends(FascicolableBaseService, _super);
        /**
         * Costruttore
         */
        function FascicolableBaseService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /**
         * Inserisce una nuova Document Unit in un Fascicolo
         * @param model
         * @param callback
         * @param error
         */
        FascicolableBaseService.prototype.insertFascicleUD = function (model, insertAction, callback, error) {
            var url = this._configuration.WebAPIUrl;
            if (insertAction && insertAction == FascicolableActionType.AutomaticDetection) {
                url = url.concat("?actionType=", InsertActionType.AutomaticIntoFascicleDetection.toString());
            }
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Modifica una Document Unit in un Fascicolo
         * @param model
         * @param callback
         * @param error
         */
        FascicolableBaseService.prototype.updateFascicleUD = function (model, updateAction, callback, error) {
            var url = this._configuration.WebAPIUrl;
            if (updateAction) {
                url = url.concat("?actionType=", updateAction.toString());
            }
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Rimuove una Document Unit da un Fascicolo
         * @param model
         * @param callback
         * @param error
         */
        FascicolableBaseService.prototype.deleteFascicleUD = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        return FascicolableBaseService;
    }(BaseService));
    return FascicolableBaseService;
});
//# sourceMappingURL=FascicolableBaseService.js.map