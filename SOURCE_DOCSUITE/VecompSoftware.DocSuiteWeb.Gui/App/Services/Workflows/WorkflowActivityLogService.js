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
define(["require", "exports", "App/Services/BaseService"], function (require, exports, BaseService) {
    var WorkflowActivityLogService = /** @class */ (function (_super) {
        __extends(WorkflowActivityLogService, _super);
        /**
        * Costruttore
        */
        function WorkflowActivityLogService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        WorkflowActivityLogService.prototype.insertWorkflowActivityLog = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        return WorkflowActivityLogService;
    }(BaseService));
    return WorkflowActivityLogService;
});
//# sourceMappingURL=WorkflowActivityLogService.js.map